namespace Application.Dtos;

/// <summary>Represents sales data for a single day.</summary>
public class DailySalesSummary
{
    public DateTime Date { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
}
