namespace TireServiceApplication.Source.Entities;

// Заказ
public class Order
{
    public string? Id { get; set; }

    public bool? Deleted { get; set; }

    public double? Cost { get; set; }

    public string? Status { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    
    public Client? ClientId { get; set; }
    public Worker? WorkerId { get; set; }

    public Service?[]? ServiceId { get; set; }
    
    public string? TitleView { get; set; }
    public string? CostView { get; set; }
    public string? StatusView { get; set; }
    public Color? StatusColorView { get; set; }
}