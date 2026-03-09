namespace Application.Services;

/// <summary>Handles shopping cart operations: add, update, remove items, and calculate totals.</summary>
public class CartService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CartService(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
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
                Quantity = request.Quantity,
                Cart = customer.Cart
            });
        }

        _unitOfWork.SaveChanges();
    }

    /// <summary>Updates the quantity of a cart item. Set NewQuantity to 0 to remove the item.</summary>
    public void UpdateCartItem(Customer customer, UpdateCartItemRequest request)
    {
        Guard.Against.Negative(request.NewQuantity, message: "Quantity cannot be negative.");

        var cartItem = customer.Cart.Items.FirstOrDefault(i => i.Product.Id == request.ProductId)
            ?? throw new InvalidOperationException("Product not found in cart.");

        if (request.NewQuantity == 0)
        {
            customer.Cart.Items.Remove(cartItem);
            _unitOfWork.SaveChanges();
            return;
        }

        var product = _productRepository.GetById(request.ProductId)
            ?? throw new InvalidOperationException($"Product with ID {request.ProductId} not found.");

        if (request.NewQuantity > product.Stock)
            throw new InvalidOperationException($"Insufficient stock. Available: {product.Stock}, Requested: {request.NewQuantity}.");

        cartItem.Quantity = request.NewQuantity;
        _unitOfWork.SaveChanges();
    }

    /// <summary>Removes a product from the customer's cart.</summary>
    public void RemoveFromCart(Customer customer, int productId)
    {
        var cartItem = customer.Cart.Items.FirstOrDefault(i => i.Product.Id == productId)
            ?? throw new InvalidOperationException("Product not found in cart.");

        customer.Cart.Items.Remove(cartItem);
        _unitOfWork.SaveChanges();
    }

    /// <summary>Returns all items in the customer's cart.</summary>
    public IEnumerable<CartItem> GetCartItems(Customer customer) => customer.Cart.Items;

    /// <summary>Calculates the total price of all items in the cart using LINQ.</summary>
    public decimal GetCartTotal(Customer customer) =>
        customer.Cart.Items.Sum(item => item.Product.Price * item.Quantity);

    /// <summary>Removes all items from the customer's cart.</summary>
    public void ClearCart(Customer customer)
    {
        customer.Cart.Items.Clear();
        _unitOfWork.SaveChanges();
    }
}
