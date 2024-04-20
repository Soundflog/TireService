using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TireService.Models;
using TireService.Services;

namespace TireService.Controllers;

// Контроллер Аутенфикации 

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly UserService _userService;
    
    public AuthenticationController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Login(string login, string password)
    {
        var users = await _userService.GetAllAdmins();
        if (!users.Any())
        {
            var newAdmin = new User
            {
                Login = login,
                Password = password,
                Role = "admin"
            };
            await _userService.CreateAsync(newAdmin);
            return GetAdminIdentity(newAdmin);
        }
        
        var identity = GetIdentity(login, password);
        if (identity.Result is null) return BadRequest("Invalid username or password.");
        
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@5665!"));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            issuer: "CodeMaze",
            audience: "https://localhost:5001",
            claims: identity.Result.Claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: signinCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        var response = new
        {
            access_token = tokenString,
            username = identity.Result.Name
        };
        return Ok(response);
    }
    
    private async Task<ClaimsIdentity?> GetIdentity(string login, string password)
    {
        var person = await _userService.GetByLoginPassword(login, password);
        if (person is not { Role: { } }) return null;
        
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, person.Id!),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role)
        };
        ClaimsIdentity? claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;

        // если пользователя не найдено
    }

    private OkObjectResult GetAdminIdentity(User newAdmin)
    {
        
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, newAdmin.Login),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, newAdmin.Role!)
        };
        ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);


        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@5665!"));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            issuer: "CodeMaze",
            audience: "https://localhost:5001",
            claims: claimsIdentity.Claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: signinCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        var response = new
        {
            access_token = tokenString,
            username = claimsIdentity.Name
        };
        
        return Ok(response);
    }
}