namespace Application.Services;

/// <summary>Handles shopping cart operations: add, update, remove items, and calculate totals.</summary>
public class CartService
{
    private readonly IProductRepository _productRepository;

    public CartService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>Adds a product to the customer's cart. Increments quantity if the product is already in the cart.</summary>
    public void AddToCart(Customer customer, AddToCartRequest request)
    {
        Guard.Against.NegativeOrZero(request.Quantity, message: "Quantity must be greater than zero.");

        var product = _productRepository.GetById(request.ProductId)
            ?? throw new InvalidOperationException($"Product with ID {request.ProductId} not found.");

        var existingItem = customer.Cart.Items.FirstOrDefault(i => i.Product.Id == request.ProductId);

        var totalQuantity = (existingItem?.Quantity ?? 0) + request.Quantity;
        if (totalQuantity > product.Stock)
            throw new InvalidOperationException($"Insufficient stock. Available: {product.Stock}, Requested: {totalQuantity}.");

        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            customer.Cart.Items.Add(new CartItem
            {
                Product = product,
                Quantity = request.Quantity
            });
        }
    }

    /// <summary>Updates the quantity of a cart item. Set NewQuantity to 0 to remove the item.</summary>
    public void UpdateCartItem(Customer customer, UpdateCartItemRequest request)
    {
        if (request.NewQuantity < 0)
            throw new ArgumentException("Quantity cannot be negative.");

        var cartItem = customer.Cart.Items.FirstOrDefault(i => i.Product.Id == request.ProductId)
            ?? throw new InvalidOperationException("Product not found in cart.");

        if (request.NewQuantity == 0)
        {
            customer.Cart.Items.Remove(cartItem);
            return;
        }

        var product = _productRepository.GetById(request.ProductId)
            ?? throw new InvalidOperationException($"Product with ID {request.ProductId} not found.");

        if (request.NewQuantity > product.Stock)
            throw new InvalidOperationException($"Insufficient stock. Available: {product.Stock}, Requested: {request.NewQuantity}.");

        cartItem.Quantity = request.NewQuantity;
    }

    /// <summary>Removes a product from the customer's cart.</summary>
    public void RemoveFromCart(Customer customer, int productId)
    {
        var cartItem = customer.Cart.Items.FirstOrDefault(i => i.Product.Id == productId)
            ?? throw new InvalidOperationException("Product not found in cart.");

        customer.Cart.Items.Remove(cartItem);
    }

    /// <summary>Returns all items in the customer's cart.</summary>
    public IEnumerable<CartItem> GetCartItems(Customer customer) => customer.Cart.Items;

    /// <summary>Calculates the total price of all items in the cart using LINQ.</summary>
    public decimal GetCartTotal(Customer customer) =>
        customer.Cart.Items.Sum(item => item.Product.Price * item.Quantity);

    /// <summary>Removes all items from the customer's cart.</summary>
    public void ClearCart(Customer customer) => customer.Cart.Items.Clear();
}
