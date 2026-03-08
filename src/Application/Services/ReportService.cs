namespace Application.Services;

/// <summary>Generates sales reports and analytics by aggregating order and payment data using LINQ.</summary>
public class ReportService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;

    public ReportService(IOrderRepository orderRepository, IPaymentRepository paymentRepository)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
    }

    /// <summary>Returns the total number of orders placed in the system.</summary>
    public int GetTotalOrders() =>
        _orderRepository.GetAll().Count();

    /// <summary>Returns the total revenue from all payments recorded.</summary>
    public decimal GetTotalRevenue() =>
        _paymentRepository.GetAll().Sum(payment => payment.Amount);

    /// <summary>Returns a count of orders grouped by their current status.</summary>
    public IEnumerable<OrderStatusSummary> GetOrdersByStatus() =>
        _orderRepository.GetAll()
            .GroupBy(order => order.Status)
            .Select(statusGroup => new OrderStatusSummary
            {
                Status = statusGroup.Key,
                Count = statusGroup.Count(),
                Total = statusGroup.Sum(order => order.Total)
            })
            .OrderByDescending(summary => summary.Count)
            .ToList();

    /// <summary>Returns the top-selling products ranked by total quantity sold across all orders.</summary>
    public IEnumerable<ProductSalesSummary> GetTopSellingProducts(int count = 5) =>
        _orderRepository.GetAll()
            .SelectMany(order => order.Items)
            .GroupBy(item => item.Product.Id)
            .Select(productGroup => new ProductSalesSummary
            {
                ProductId = productGroup.Key,
                ProductName = productGroup.First().Product.Name,
                TotalQuantitySold = productGroup.Sum(item => item.Quantity),
                TotalRevenue = productGroup.Sum(item => item.UnitPrice * item.Quantity)
            })
            .OrderByDescending(product => product.TotalQuantitySold)
            .Take(count)
            .ToList();

    /// <summary>Returns the average order value across all orders.</summary>
    public decimal GetAverageOrderValue()
    {
        var orders = _orderRepository.GetAll().ToList();
        return orders.Any() ? orders.Average(order => order.Total) : 0;
    }

    /// <summary>Returns a daily sales breakdown ordered by date descending.</summary>
    public IEnumerable<DailySalesSummary> GetDailySales() =>
        _orderRepository.GetAll()
            .GroupBy(order => order.PlacedAt.Date)
            .Select(dateGroup => new DailySalesSummary
            {
                Date = dateGroup.Key,
                OrderCount = dateGroup.Count(),
                Revenue = dateGroup.Sum(order => order.Total)
            })
            .OrderByDescending(daily => daily.Date)
            .ToList();
}
