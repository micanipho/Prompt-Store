namespace Application.Dtos;

public class UpdateOrderStatusRequest
{
    public int OrderId { get; set; }
    public OrderStatus NewStatus { get; set; }
}
