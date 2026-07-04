namespace BasketManagement.Application.Basket.DTOs;

public class BasketDTO
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<BasketItemDTO> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}