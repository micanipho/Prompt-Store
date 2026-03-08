namespace Infrastructure.Repositories;

/// <summary>In-memory implementation of IOrderRepository using a list.</summary>
public class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = [];
    private int _nextId = 1;

    /// <inheritdoc/>
    public void Add(Order order)
    {
        order.Id = _nextId++;
        _orders.Add(order);
    }

    /// <inheritdoc/>
    public Order? GetById(int id) => _orders.FirstOrDefault(o => o.Id == id);

    /// <inheritdoc/>
    public IEnumerable<Order> GetAll() => _orders.ToList();
}
