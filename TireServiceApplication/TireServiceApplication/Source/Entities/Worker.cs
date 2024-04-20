#nullable enable
namespace TireServiceApplication.Source.Entities;

// Сотрудник
public class Worker
{
    public string? Id { get; set; }
    public bool? Deleted { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MidName { get; set; }
    public DateTime? DateOfBirthday { get; set; }
    public string? NumberPhone { get; set; }
    public Branch? BranchId { get; set; }
    public double? PayPerHour { get; set; }
    
    public string? TitleView { get; set; }
    public string? NumberPhoneView { get; set; }
    public string? PayPerHourView { get; set; }
}