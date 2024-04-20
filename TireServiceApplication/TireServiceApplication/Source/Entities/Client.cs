namespace TireServiceApplication.Source.Entities;

public class Client
{
    public string? Id { get; set; }
    
    public bool? Deleted { get; set; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public string? MidName { get; set; }
    
    public string? NumberPhone { get; set; }

    public Car[]? CarId { get; set; }

    public double? TotalSpent { get; set; }
    
    public string? TitleView { get; set; }
    
    public string? TotalSpentView { get; set; }
}