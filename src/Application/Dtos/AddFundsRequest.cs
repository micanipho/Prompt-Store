namespace Application.Dtos;

/// <summary>Request to add funds to a customer's wallet.</summary>
public class AddFundsRequest
{
    public decimal Amount { get; set; }
}
