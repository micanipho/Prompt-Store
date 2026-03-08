namespace Application.Dtos;

/// <summary>Carries the data needed to change an order's status.</summary>
public class UpdateOrderStatusRequest
{
    public int OrderId { get; set; }
    public OrderStatus NewStatus { get; set; }
}
