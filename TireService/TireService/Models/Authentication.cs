using System.ComponentModel.DataAnnotations;

namespace TireService.Models;
// Авторизация
public class Authentication
{
    [Required(ErrorMessage = "Login Required")]
    public string Login { get; set; } = null!;
    
    [Required(ErrorMessage = "Password Required")]
    public string Password { get; set; } = null!;
    
}