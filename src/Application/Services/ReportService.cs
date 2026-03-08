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
        _paymentRepository.GetAll().Sum(p => p.Amount);

    /// <summary>Returns a count of orders grouped by their current status.</summary>
    public IEnumerable<OrderStatusSummary> GetOrdersByStatus() =>
        _orderRepository.GetAll()
            .GroupBy(o => o.Status)
            .Select(g => new OrderStatusSummary
            {
                Status = g.Key,
                Count = g.Count(),
                Total = g.Sum(o => o.Total)
            })
            .OrderByDescending(s => s.Count)
            .ToList();

    /// <summary>Returns the top-selling products ranked by total quantity sold across all orders.</summary>
    public IEnumerable<ProductSalesSummary> GetTopSellingProducts(int count = 5) =>
        _orderRepository.GetAll()
            .SelectMany(o => o.Items)
            .GroupBy(item => item.Product.Id)
            .Select(g => new ProductSalesSummary
            {
                ProductId = g.Key,
                ProductName = g.First().Product.Name,
                TotalQuantitySold = g.Sum(item => item.Quantity),
                TotalRevenue = g.Sum(item => item.UnitPrice * item.Quantity)
            })
            .OrderByDescending(p => p.TotalQuantitySold)
            .Take(count)
            .ToList();

    /// <summary>Returns the average order value across all orders.</summary>
    public decimal GetAverageOrderValue()
    {
        var orders = _orderRepository.GetAll().ToList();
        return orders.Any() ? orders.Average(o => o.Total) : 0;
    }

    /// <summary>Returns a daily sales breakdown ordered by date descending.</summary>
    public IEnumerable<DailySalesSummary> GetDailySales() =>
        _orderRepository.GetAll()
            .GroupBy(o => o.PlacedAt.Date)
            .Select(g => new DailySalesSummary
            {
                Date = g.Key,
                OrderCount = g.Count(),
                Revenue = g.Sum(o => o.Total)
            })
            .OrderByDescending(d => d.Date)
            .ToList();
}

/// <summary>Represents a summary of orders grouped by status.</summary>
public class OrderStatusSummary
{
    public OrderStatus Status { get; set; }
    public int Count { get; set; }
    public decimal Total { get; set; }
}

/// <summary>Represents sales data for a single product.</summary>
public class ProductSalesSummary
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}

/// <summary>Represents sales data for a single day.</summary>
public class DailySalesSummary
{
    public DateTime Date { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
}
