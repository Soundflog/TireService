using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TireService.Models;
using TireService.Services;

namespace TireService.Controllers;

// Котроллер Филиала

[ApiController]
[Route("api/[controller]")]
public class BranchController : ControllerBase
{
    private readonly BranchService _branchService;

    public BranchController(BranchService branchService) =>
        _branchService = branchService;

    [HttpGet, Authorize(Roles = "admin")]
    public async Task<List<Branch>> GetAll() =>
        await _branchService.GetAllAsync();

    [HttpPost, Authorize(Roles = "admin")]
    public async Task<IActionResult> Post(Branch newBranch)
    {
        newBranch.Deleted = false;
        await _branchService.CreateAsync(newBranch);

        return CreatedAtAction(nameof(GetAll), new { id = newBranch.Id }, newBranch);
    }

    [HttpPut, Authorize(Roles = "admin")]
    public async Task<IActionResult> Update( Branch updatedBranch)
    {
        var id = updatedBranch.Id;
        if (id == null) return NotFound("WRONG: Id не указано");
        var branch = await _branchService.GetAsync(id);

        if (branch is null)
        {
            return NotFound();
        }

        updatedBranch.Id = branch.Id;
        updatedBranch.Deleted = false;
        await _branchService.UpdateAsync(id, updatedBranch);
        
        return NoContent();
    }

    [HttpDelete("{id:length(24)}"), Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var branch = await _branchService.GetAsync(id);

        if (branch is null || branch.Deleted == true)
        {
            return NotFound();
        }
        // Меняем поле на удаленное, а запись перезаписываем
        branch.Deleted = true;
        await _branchService.UpdateAsync(id, branch);

        return NoContent();
    }
}