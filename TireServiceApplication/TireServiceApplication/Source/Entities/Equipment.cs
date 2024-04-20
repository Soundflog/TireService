namespace TireServiceApplication.Source.Entities;

// Оборудование
public class Equipment
{
    public string? Id { get; set; }

    public bool? Deleted { get; set; }

    public string? Title { get; set; }

    public double? Cost { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public DateTime? BuildDate { get; set; }

    public DateTime? GuaranteeDate { get; set; }

    public int? UsefulLifeDate { get; set; }

    public Branch? BranchId { get; set; }
}