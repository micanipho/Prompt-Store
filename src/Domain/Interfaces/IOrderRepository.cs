namespace Domain.Interfaces;

/// <summary>Defines the contract for order persistence operations.</summary>
public interface IOrderRepository
{
    /// <summary>Adds a new order and assigns it a unique ID.</summary>
    void Add(Order order);

    /// <summary>Returns the order with the given ID, or null if not found.</summary>
    Order? GetById(int id);

    /// <summary>Returns all orders in the system.</summary>
    IEnumerable<Order> GetAll();
}
