namespace TireServiceApplication.Source.Entities;

// Пользователь
public class User
{
    public string? Id { get; set; }

    public bool? Deleted { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public Branch? BranchId { get; set; }
    
    public string? RoleView { get; set; }
}