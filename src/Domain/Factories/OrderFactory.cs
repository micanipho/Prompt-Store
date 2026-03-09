namespace Domain.Factories;

using Domain.Entities;
using Domain.Enums;

/// <summary>Factory for creating Order and OrderItem entities.</summary>
public interface IOrderFactory
{
    /// <summary>Creates a new Order entity with its associated OrderItems from a Customer's Cart.</summary>
    Order CreateOrder(Customer customer, decimal totalAmount);
}

/// <summary>Implementation of the OrderFactory.</summary>
public class OrderFactory : IOrderFactory
{
    public Order CreateOrder(Customer customer, decimal totalAmount)
    {
        var orderItems = customer.Cart.Items.Select(cartItem => new OrderItem
        {
            Product = cartItem.Product,
            Quantity = cartItem.Quantity,
            UnitPrice = cartItem.Product.Price
        }).ToList();

        return new Order
        {
            Items = orderItems,
            Total = totalAmount,
            Status = OrderStatus.Pending,
            PlacedAt = DateTime.Now,
            Customer = customer
        };
    }
}
