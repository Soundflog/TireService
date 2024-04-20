using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TireService.Models;
using TireService.Services;

namespace TireService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly BranchService _branchService;

    public UserController(UserService service, BranchService branchService)
    {
        _userService = service;
        _branchService = branchService;
    }

    [HttpGet, Authorize(Roles = "admin")]
    public async Task<List<User>> GetAll() =>
        await _userService.GetAllAsync();

    [HttpGet, Authorize(Roles = "admin")]
    [Route("getAllExists")]
    public async Task<List<User>> GetBrings() =>
        await _userService.GetAllBring();

    [HttpGet, Authorize(Roles = "admin, manager")]
    [Route("getYourself")]
    public async Task<User?> GetMe()
    {
        var username = User.Identity!.Name;
        if (username == null) return null!;
        var user = await _userService.GetAsync(username);

        if (user != null) return user;
        return null;
    }
    
    [HttpGet, Authorize(Roles = "admin, manager")]
    [Route("getOneByBranch")]
    public async Task<User?> GetOneByBranch(string id)
    {
        var username = User.Identity!.Name;
        if (username == null) return null!;
        var user = await _userService.GetAsync(username);
        if (user!.BranchId?.Id == null) return null!;
        
        return await _userService.GetOneByBranch(id, user.BranchId.Id);
    }
    

    [HttpPost, Authorize(Roles = "admin")]
    public async Task<IActionResult> Post(User newUser)
    {
        var loginNewUser = newUser.Login;
        var similarUser = await _userService.GetByLogin(loginNewUser);
        if (similarUser is not null)
            return NotFound("Такой логин уже занят");
        if (newUser.BranchId?.Id == null)
            return NotFound("Такого филиала не существует");
        
        var branch = await _branchService.GetAsync(newUser.BranchId.Id);
        newUser.BranchId = branch;
        newUser.Deleted = false;
        newUser.Role = newUser.Role switch
        {
            null => "manager",
            "admin" => newUser.Role,
            _ => "manager"
        };
        await _userService.CreateAsync(newUser);

        return CreatedAtAction(nameof(GetAll), new { id = newUser.Id }, newUser);
    }

    [HttpPut, Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(User updatedUser)
    {
        var id = updatedUser.Id;
        if (id == null) return NotFound("WRONG: ID не указано");
        
        var user = await _userService.GetAsync(id);
        
        if (user is null || updatedUser.BranchId?.Id == null)
            return NotFound();

        updatedUser.Id = user.Id;
        var branch = await _branchService.GetAsync(updatedUser.BranchId.Id);
        updatedUser.BranchId = branch;
        await _userService.UpdateAsync(id, updatedUser);

        return NoContent();
    }
    
    [HttpDelete("{id:length(24)}"), Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userService.GetAsync(id);

        if (user is null || user.Deleted == true)
        {
            return NotFound();
        }
        // Меняем поле на удаленное, а запись перезаписываем
        user.Deleted = true;
        await _userService.UpdateAsync(id, user);

        return NoContent();
    }
}