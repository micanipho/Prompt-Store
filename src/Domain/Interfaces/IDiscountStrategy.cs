namespace Domain.Interfaces;

using Domain.Entities;

/// <summary>Strategy interface for calculating discounts on an order.</summary>
public interface IDiscountStrategy
{
    /// <summary>The display name of the discount type.</summary>
    string Name { get; }

    /// <summary>Calculates the final total after applying the discount.</summary>
    decimal CalculateTotal(decimal subtotal);
}
