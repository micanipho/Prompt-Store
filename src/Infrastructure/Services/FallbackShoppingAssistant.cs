using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Services;

/// <summary>
/// Rule-based fallback assistant used when the Claude API is unavailable or no API key is configured.
/// Supports both informational queries and live shopping actions (add/remove/update cart, checkout).
/// </summary>
public sealed class FallbackShoppingAssistant : IShoppingAssistant
{
    private readonly CartService _cartService;
    private readonly OrderService _orderService;

    public FallbackShoppingAssistant(CartService cartService, OrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }

    public Task<string> GetResponseAsync(
        string userMessage,
        ShoppingContext context,
        IList<ChatMessage> conversationHistory,
        CancellationToken cancellationToken = default)
    {
        var msg = userMessage.ToLowerInvariant();
        return Task.FromResult(Dispatch(msg, context));
    }

    // ─── Intent dispatch ─────────────────────────────────────────────────────

    private string Dispatch(string msg, ShoppingContext ctx) =>
        DispatchAction(msg, ctx) ?? DispatchInformational(msg, ctx) ?? HelpResponse();

    /// <summary>Returns a response if the message matches a mutating cart/order action; otherwise null.</summary>
    private string? DispatchAction(string msg, ShoppingContext ctx)
    {
        if (Is(msg, "checkout", "place order", "complete order", "confirm order", "pay now", "buy now"))
            return TryCheckout(ctx);

        if (Is(msg, "clear cart", "empty cart", "remove all from cart", "delete everything from cart"))
            return TryClearCart(ctx);

        if ((Is(msg, "remove", "delete", "take out", "drop") && HasCartRef(msg)) ||
            (Is(msg, "remove") && HasProductRef(msg, ctx)))
            return TryRemoveFromCart(msg, ctx);

        if (Is(msg, "add", "put", "place") && (HasCartRef(msg) || HasProductRef(msg, ctx)))
            return TryAddToCart(msg, ctx);

        if (IsUpdateQuantityIntent(msg, ctx))
            return TryUpdateQuantity(msg, ctx);

        return null;
    }

    private static bool IsUpdateQuantityIntent(string msg, ShoppingContext ctx) =>
        (Is(msg, "update", "change", "set", "modify") && Is(msg, "quantity", "amount", "qty")) ||
        (Is(msg, "update", "change", "set") && Is(msg, "to ") && HasProductRef(msg, ctx));

    /// <summary>Returns a response for read-only queries; otherwise null.</summary>
    private static string? DispatchInformational(string msg, ShoppingContext ctx)
    {
        if (Is(msg, "wallet", "balance", "how much money", "funds"))
            return WalletResponse(ctx);

        if (Is(msg, "order", "track", "status") && !Is(msg, "place", "checkout"))
            return OrderResponse(ctx);

        if (Is(msg, "cart", "basket", "bag") && !Is(msg, "add", "put", "remove", "clear"))
            return CartResponse(ctx);

        if (Is(msg, "under r", "less than r", "below r", "cheaper than r"))
        {
            var price = ExtractPrice(msg);
            if (decimal.TryParse(price, out var maxPrice))
                return PriceRangeResponse(ctx, maxPrice);
        }

        if (Is(msg, "compar", "cheapest", "most expensive", "price range"))
            return CompareResponse(ctx);

        var catMatch = ctx.Products
            .Select(p => p.Category)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(c => msg.Contains(c.ToLowerInvariant()));

        if (catMatch != null)
            return CategoryResponse(ctx, catMatch);

        if (Is(msg, "recommend", "suggest", "find", "search", "browse", "show", "product", "list"))
            return RecommendationResponse(ctx);

        return null;
    }

    // ─── Cart mutations ───────────────────────────────────────────────────────

    private string TryAddToCart(string msg, ShoppingContext ctx)
    {
        var (product, usedId) = FindProduct(msg, ctx);
        if (product == null)
            return "I couldn't identify that product. Try \"show products\" to see what's available, then say something like \"add laptop to cart\" or \"add id 3 to cart\".";

        var qty = ExtractQuantity(msg, usedId);

        try
        {
            _cartService.AddToCart(ctx.Customer, new AddToCartRequest { ProductId = product.Id, Quantity = qty });
            var newTotal = ctx.Customer.Cart.Items.Sum(i => i.Product.Price * i.Quantity);
            return $"Added {qty}x {product.Name} (R{product.Price:F2} each) to your cart.\nCart total: R{newTotal:F2}";
        }
        catch (Exception ex)
        {
            return $"Couldn't add to cart: {ex.Message}";
        }
    }

    private string TryRemoveFromCart(string msg, ShoppingContext ctx)
    {
        var product = FindProductInCart(msg, ctx);
        if (product == null)
            return "I couldn't find that product in your cart. Say \"what's in my cart\" to see your items.";

        try
        {
            _cartService.UpdateCartItem(ctx.Customer, new UpdateCartItemRequest { ProductId = product.Id, NewQuantity = 0 });
            return $"Removed {product.Name} from your cart.";
        }
        catch (Exception ex)
        {
            return $"Couldn't remove from cart: {ex.Message}";
        }
    }

    private string TryUpdateQuantity(string msg, ShoppingContext ctx)
    {
        var product = FindProductInCart(msg, ctx);
        if (product == null)
            return "I couldn't find that product in your cart. Say \"what's in my cart\" to see your items.";

        // "to X", "= X", or "quantity X" take priority; fall back to first standalone number
        var toMatch = Regex.Match(msg, @"(?:to|=|quantity|qty)\s*(\d+)");
        if (!toMatch.Success || !int.TryParse(toMatch.Groups[1].Value, out var newQty))
            newQty = ExtractQuantity(msg);

        try
        {
            _cartService.UpdateCartItem(ctx.Customer, new UpdateCartItemRequest { ProductId = product.Id, NewQuantity = newQty });
            return newQty == 0
                ? $"Removed {product.Name} from your cart."
                : $"Updated {product.Name} quantity to {newQty}.";
        }
        catch (Exception ex)
        {
            return $"Couldn't update cart: {ex.Message}";
        }
    }

    private string TryClearCart(ShoppingContext ctx)
    {
        if (!ctx.Customer.Cart.Items.Any())
            return "Your cart is already empty.";

        try
        {
            _cartService.ClearCart(ctx.Customer);
            return "Your cart has been cleared.";
        }
        catch (Exception ex)
        {
            return $"Couldn't clear cart: {ex.Message}";
        }
    }

    private string TryCheckout(ShoppingContext ctx)
    {
        if (!ctx.Customer.Cart.Items.Any())
            return "Your cart is empty. Add some products before checking out!";

        var total = ctx.Customer.Cart.Items.Sum(i => i.Product.Price * i.Quantity);
        var balance = ctx.Customer.Balance;

        if (balance < total)
            return $"Insufficient wallet balance. Cart total: R{total:F2} | Wallet: R{balance:F2}\nAdd funds via the 'Add Wallet Funds' menu option.";

        try
        {
            var order = _orderService.PlaceOrder(ctx.Customer);
            return $"Order #{order.Id} placed! R{order.Total:F2} deducted. New wallet balance: R{ctx.Customer.Balance:F2}.";
        }
        catch (Exception ex)
        {
            return $"Checkout failed: {ex.Message}";
        }
    }

    // ─── Informational responses ──────────────────────────────────────────────

    private static string WalletResponse(ShoppingContext ctx) =>
        $"Your current wallet balance is R{ctx.Customer.Balance:F2}.\nAdd funds via 'Add Wallet Funds' in the menu.";

    private static string OrderResponse(ShoppingContext ctx)
    {
        if (!ctx.RecentOrders.Any())
            return "You don't have any recent orders yet. Start shopping to place your first order!";

        var sb = new StringBuilder("Your recent orders:\n\n");
        foreach (var o in ctx.RecentOrders)
            sb.AppendLine($"  Order #{o.Id} | {o.PlacedAt:yyyy-MM-dd} | {o.Status} | R{o.Total:F2} ({o.Items.Count} item(s))");
        sb.AppendLine("\nFor full details, use 'Track Orders' in the menu.");
        return sb.ToString();
    }

    private static string CartResponse(ShoppingContext ctx)
    {
        if (!ctx.CartItems.Any())
            return "Your cart is empty. Ask me to add something — e.g. \"add laptop to cart\".";

        var total = ctx.CartItems.Sum(i => i.Product.Price * i.Quantity);
        var sb = new StringBuilder($"Your cart ({ctx.CartItems.Count} item(s)) — Total: R{total:F2}\n\n");
        foreach (var item in ctx.CartItems)
            sb.AppendLine($"  ID:{item.Product.Id} | {item.Product.Name} x{item.Quantity} @ R{item.Product.Price:F2} = R{item.Product.Price * item.Quantity:F2}");
        return sb.ToString();
    }

    private static string PriceRangeResponse(ShoppingContext ctx, decimal maxPrice)
    {
        var matches = ctx.Products.Where(p => p.Price <= maxPrice && p.Stock > 0)
                                  .OrderBy(p => p.Price).Take(6).ToList();
        if (!matches.Any())
            return $"No products found under R{maxPrice:F2} that are currently in stock.";

        var sb = new StringBuilder($"Products available under R{maxPrice:F2}:\n\n");
        foreach (var p in matches)
            sb.AppendLine($"  ID:{p.Id} | {p.Name} | {p.Category} | R{p.Price:F2}");
        sb.AppendLine("\nSay \"add [name] to cart\" to add one.");
        return sb.ToString();
    }

    private static string CategoryResponse(ShoppingContext ctx, string category)
    {
        var matches = ctx.Products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase) && p.Stock > 0)
                                  .OrderBy(p => p.Price).Take(6).ToList();
        if (!matches.Any())
            return $"No '{category}' products are currently in stock.";

        var sb = new StringBuilder($"Products in '{category}':\n\n");
        foreach (var p in matches)
            sb.AppendLine($"  ID:{p.Id} | {p.Name} | R{p.Price:F2} | {p.Description}");
        sb.AppendLine("\nSay \"add [name] to cart\" to add one.");
        return sb.ToString();
    }

    private static string RecommendationResponse(ShoppingContext ctx)
    {
        var inStock = ctx.Products.Where(p => p.Stock > 0).OrderBy(p => p.Price).Take(6).ToList();
        if (!inStock.Any())
            return "There are no products currently in stock. Check back soon!";

        var sb = new StringBuilder("Here are some products you might like:\n\n");
        foreach (var p in inStock)
            sb.AppendLine($"  ID:{p.Id} | {p.Name} | {p.Category} | R{p.Price:F2}");
        sb.AppendLine("\nSay \"add [name] to cart\" to add one.");
        return sb.ToString();
    }

    private static string CompareResponse(ShoppingContext ctx)
    {
        var categories = ctx.Products.Select(p => p.Category).Distinct().ToList();
        if (!categories.Any())
            return "No products available to compare.";

        var sb = new StringBuilder("Price overview by category:\n\n");
        foreach (var cat in categories)
        {
            var ps = ctx.Products.Where(p => p.Category == cat && p.Stock > 0).OrderBy(p => p.Price).ToList();
            if (!ps.Any()) continue;
            sb.AppendLine($"  {cat}: R{ps.First().Price:F2} – R{ps.Last().Price:F2} ({ps.Count} item(s))");
        }
        return sb.ToString();
    }

    private static string HelpResponse() =>
        "I can help you with:\n\n" +
        "  Cart Actions:\n" +
        "    - \"Add laptop to cart\" / \"Add 2 of product id 3\"\n" +
        "    - \"Remove mouse from cart\"\n" +
        "    - \"Set laptop quantity to 3\"\n" +
        "    - \"Clear my cart\"\n" +
        "    - \"Checkout\" / \"Place my order\"\n\n" +
        "  Information:\n" +
        "    - \"Show me electronics under R500\"\n" +
        "    - \"What's in my cart?\"\n" +
        "    - \"Track my orders\"\n" +
        "    - \"Check my wallet balance\"\n" +
        "    - \"Compare prices\"\n" +
        "    - \"Recommend something cheap\"";

    // ─── Parsing helpers ──────────────────────────────────────────────────────

    private static bool Is(string msg, params string[] keywords) =>
        keywords.Any(k => msg.Contains(k));

    private static bool HasCartRef(string msg) =>
        Is(msg, "cart", "basket", "bag");

    private static bool HasProductRef(string msg, ShoppingContext ctx)
    {
        if (Regex.IsMatch(msg, @"\bid\s*\d+|\b#\d+|(?:product|item)\s+\d+")) return true;
        return ctx.Products.Any(p => NameScore(msg, p.Name) > 0);
    }

    /// <summary>Finds the best-matching product from the catalog using ID patterns first, then fuzzy name match.</summary>
    private static (Product? product, int? usedId) FindProduct(string msg, ShoppingContext ctx)
    {
        // "id X" or "#X"
        var m = Regex.Match(msg, @"\bid\s*(\d+)|#(\d+)");
        if (m.Success)
        {
            var raw = m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
            if (int.TryParse(raw, out var id))
                return (ctx.Products.FirstOrDefault(p => p.Id == id), id);
        }

        // "product X" or "item X"
        m = Regex.Match(msg, @"(?:product|item)\s+(\d+)");
        if (m.Success && int.TryParse(m.Groups[1].Value, out var pid))
            return (ctx.Products.FirstOrDefault(p => p.Id == pid), pid);

        // Fuzzy name match
        var best = ctx.Products
            .Select(p => (p, score: NameScore(msg, p.Name)))
            .Where(x => x.score > 0)
            .OrderByDescending(x => x.score)
            .FirstOrDefault();

        return (best.p, null);
    }

    /// <summary>Finds the best-matching product already in the customer's cart.</summary>
    private static Product? FindProductInCart(string msg, ShoppingContext ctx)
    {
        var m = Regex.Match(msg, @"\bid\s*(\d+)|#(\d+)");
        if (m.Success)
        {
            var raw = m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
            if (int.TryParse(raw, out var id))
                return ctx.CartItems.FirstOrDefault(ci => ci.Product.Id == id)?.Product;
        }

        m = Regex.Match(msg, @"(?:product|item)\s+(\d+)");
        if (m.Success && int.TryParse(m.Groups[1].Value, out var pid))
            return ctx.CartItems.FirstOrDefault(ci => ci.Product.Id == pid)?.Product;

        return ctx.CartItems
            .Select(ci => (ci.Product, score: NameScore(msg, ci.Product.Name)))
            .Where(x => x.score > 0)
            .OrderByDescending(x => x.score)
            .FirstOrDefault().Product;
    }

    /// <summary>Scores a product name against the message by counting significant matching words.</summary>
    private static int NameScore(string msg, string productName)
    {
        var words = productName.ToLowerInvariant()
                               .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                               .Where(w => w.Length > 2);
        return words.Count(msg.Contains);
    }

    /// <summary>
    /// Extracts the first positive integer from the message, optionally skipping a known product ID
    /// so that "add product 5 to cart" doesn't confuse the product ID with a quantity.
    /// </summary>
    private static int ExtractQuantity(string msg, int? excludeId = null)
    {
        foreach (Match m in Regex.Matches(msg, @"\b(\d+)\b"))
        {
            if (!int.TryParse(m.Value, out var n) || n <= 0 || n >= 1000) continue;
            if (excludeId.HasValue && n == excludeId.Value) continue;
            return n;
        }
        return 1;
    }

    private static string ExtractPrice(string msg)
    {
        var sb = new StringBuilder();
        var foundR = false;
        foreach (var c in msg)
        {
            if (c == 'r') { foundR = true; continue; }
            if (foundR && (char.IsDigit(c) || c == '.')) sb.Append(c);
            else if (foundR && sb.Length > 0) break;
        }
        return sb.ToString();
    }
}
