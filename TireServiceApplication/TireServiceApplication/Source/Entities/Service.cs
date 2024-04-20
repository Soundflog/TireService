namespace TireServiceApplication.Source.Entities;

// Услуга
public class Service
{
    public string? Id { get; set; }
    public bool? Deleted { get; set; }
    public string? Title { get; set; }
    public double? Cost { get; set; }
    public int? DurationInMinutes { get; set; }
    public Branch? BranchId { get; set; }
    public Equipment?[]? EquipmentId { get; set; }
    public string? CostView { get;set; }
    public string? DurationInMinutesView { get;set; }
}