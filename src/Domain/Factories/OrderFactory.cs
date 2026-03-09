using Domain.Interfaces;

namespace Domain.Factories;

using Domain.Entities;
using Domain.Enums;

/// <summary>Factory for creating Order and OrderItem entities.</summary>
public interface IOrderFactory
{
    /// <summary>Creates a new Order entity with its associated OrderItems from a Customer's Cart.</summary>
    Order CreateOrder(Customer customer, decimal subtotal, IDiscountStrategy discountStrategy);
}

/// <summary>Implementation of the OrderFactory.</summary>
public class OrderFactory : IOrderFactory
{
    public Order CreateOrder(Customer customer, decimal subtotal, IDiscountStrategy discountStrategy)
    {
        var orderItems = customer.Cart.Items.Select(cartItem => new OrderItem
        {
            Product = cartItem.Product,
            Quantity = cartItem.Quantity,
            UnitPrice = cartItem.Product.Price
        }).ToList();

        var finalTotal = discountStrategy.CalculateTotal(subtotal);

        return new Order
        {
            Items = orderItems,
            Total = finalTotal,
            DiscountApplied = discountStrategy.Name,
            Status = OrderStatus.Pending,
            PlacedAt = DateTime.Now,
            Customer = customer
        };
    }
}
