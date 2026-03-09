namespace Domain.Strategies;

using Domain.Interfaces;

/// <summary>Default strategy that applies no discount.</summary>
public class NoDiscountStrategy : IDiscountStrategy
{
    public string Name => "No Discount";
    public decimal CalculateTotal(decimal subtotal) => subtotal;
}

/// <summary>Strategy that applies a percentage-based discount (e.g., 10% off).</summary>
public class PercentageDiscountStrategy(string name, decimal percentage) : IDiscountStrategy
{
    public string Name { get; } = name;
    private readonly decimal _percentage = percentage; // e.g., 0.10m for 10%

    public decimal CalculateTotal(decimal subtotal)
    {
        return subtotal * (1 - _percentage);
    }
}
