namespace Application.Dtos;

/// <summary>Request to submit a review and rating for a purchased product.</summary>
public class SubmitReviewRequest
{
    public int ProductId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}
