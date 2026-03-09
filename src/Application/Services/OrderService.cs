using Domain.Factories;
using Domain.Interfaces;
using Domain.Strategies;

namespace Application.Services;

/// <summary>Handles order placement, retrieval, and status management.</summary>
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderFactory _orderFactory;
    private readonly IDiscountStrategy _discountStrategy;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IPaymentRepository paymentRepository, IUnitOfWork unitOfWork, IOrderFactory orderFactory, IDiscountStrategy? discountStrategy = null)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _orderFactory = orderFactory;
        _discountStrategy = discountStrategy ?? new NoDiscountStrategy();
    }

    /// <summary>
    /// Places an order from the customer's current cart. Validates balance and stock,
    /// deducts funds, reduces product stock, clears the cart, and records the order.
    /// </summary>
    public Order PlaceOrder(Customer customer)
    {
        if (!customer.Cart.Items.Any())
            throw new InvalidOperationException("Cannot place an order with an empty cart.");

        var subtotal = customer.Cart.Items.Sum(item => item.Product.Price * item.Quantity);
        var total = _discountStrategy.CalculateTotal(subtotal);

        if (customer.Balance < total)
            throw new InvalidOperationException(
                $"Insufficient wallet balance. Required: R{total:F2} (after {_discountStrategy.Name}), Available: R{customer.Balance:F2}.");

        // Validate stock for all items before committing
        foreach (var cartItem in customer.Cart.Items)
        {
            var product = _productRepository.GetById(cartItem.Product.Id)
                ?? throw new InvalidOperationException($"Product '{cartItem.Product.Name}' is no longer available.");

            if (cartItem.Quantity > product.Stock)
                throw new InvalidOperationException(
                    $"Insufficient stock for '{product.Name}'. Available: {product.Stock}, Requested: {cartItem.Quantity}.");
        }

        var order = _orderFactory.CreateOrder(customer, subtotal, _discountStrategy);

        // Deduct stock from each product
        foreach (var cartItem in customer.Cart.Items)
        {
            var product = _productRepository.GetById(cartItem.Product.Id)!;
            product.Stock -= cartItem.Quantity;
            _productRepository.Update(product);
        }

        _orderRepository.Add(order);
        customer.OrderHistory.Add(order);
        customer.DeductFunds(total);
        customer.Cart.Items.Clear();

        // Record the payment transaction
        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = total,
            PaidAt = DateTime.Now
        };
        _paymentRepository.Add(payment);

        _unitOfWork.SaveChanges();

        return order;
    }

    /// <summary>Returns the order history for a given customer using LINQ.</summary>
    public IEnumerable<Order> GetOrderHistory(Customer customer) =>
        customer.OrderHistory.OrderByDescending(o => o.PlacedAt).ToList();

    /// <summary>Returns all orders in the system, sorted most recent first.</summary>
    public IEnumerable<Order> GetAllOrders() =>
        _orderRepository.GetAll().OrderByDescending(o => o.PlacedAt).ToList();

    /// <summary>Returns a single order by ID, or throws if not found.</summary>
    public Order GetOrderById(int orderId) =>
        _orderRepository.GetById(orderId)
            ?? throw new InvalidOperationException($"Order with ID {orderId} not found.");

    /// <summary>Updates the status of an existing order.</summary>
    public void UpdateOrderStatus(UpdateOrderStatusRequest request)
    {
        var order = _orderRepository.GetById(request.OrderId)
            ?? throw new InvalidOperationException($"Order with ID {request.OrderId} not found.");

        order.Status = request.NewStatus;
        _unitOfWork.SaveChanges();
    }
}
