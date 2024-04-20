using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TireService.Models;
using TireService.Services;

namespace TireService.Controllers;

// Котроллер Сотрудник

[ApiController]
[Route("api/[controller]")]
public class WorkerController : ControllerBase
{
    private readonly WorkerService _workerService;
    private readonly BranchService _branchService;
    private readonly UserService _userService;

    public WorkerController(WorkerService workerService, BranchService branchService, UserService userService)
    {
        _workerService = workerService;
        _branchService = branchService;
        _userService = userService;
    }
    
    [HttpGet, Authorize(Roles = "admin")]
    public async Task<List<Worker>> GetAll()
    {
        return await _workerService.GetAllAsync();
    }

    // Фильтр по филлиалу
    [HttpGet, Authorize(Roles = "admin, manager")]
    [Route("getAllByBranch")]
    public async Task<List<Worker>?> GetByBranch()
    {
        var username = User.Identity!.Name;
        if (username == null) return null;
        
        var user = await _userService.GetAsync(username);
        
        if (user?.BranchId?.Id == null) return null;
        
        return await _workerService.GetByBranch(user.BranchId.Id);
    }

    [HttpPost, Authorize(Roles = "admin, manager")]
    public async Task<IActionResult> Post(Worker newWorker)
    {
        var username = User.Identity!.Name;
        if (username == null) return NotFound("username not found");
        
        var user = await _userService.GetAsync(username);
        
        if (user?.BranchId?.Id == null) return NotFound("User does not have id");

        var branch = await _branchService.GetAsync(user.BranchId.Id);
        
        newWorker.BranchId = branch;
        await _workerService.CreateAsync(newWorker);
        return CreatedAtAction(nameof(GetAll), new { id = newWorker.Id }, newWorker);
    }

    [HttpPut, Authorize(Roles = "admin, manager")]
    public async Task<IActionResult> Update( Worker updatedWorker)
    {
        var id = updatedWorker.Id;
        if (id == null) return NotFound("WRONG: Id не указано");
        
        var worker = await _workerService.GetAsync(id);

        if (worker is null || worker.Deleted == true)
        {
            return NotFound("Невозможно изменить сотрудника. " +
                            "\n\t Запись удалена или не найдена");
        }
        updatedWorker.Id = worker.Id;

        /*
          Проверка поля Филиала на заполненость и не удалена ли запись,
         если нет, то оставляем значение таким какое было,
         если да, то проверяем есть ли такой филиал по id и заполняем
         */
        var username = User.Identity!.Name;
        if (username == null) return NotFound("username not found");
        var user = await _userService.GetAsync(username);
        if (user?.BranchId?.Id == null) return NotFound("User does not have id");
        var branch = await _branchService.GetAsync(user.BranchId.Id);
        
        updatedWorker.BranchId = branch;
        
        await _workerService.UpdateAsync(id, updatedWorker);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}"), Authorize(Roles = "admin, manager")]
    public async Task<IActionResult> Delete(string id)
    {
        var worker = await _workerService.GetAsync(id);

        if (worker is null || worker.Deleted == true)
        {
            return NotFound("Невозможно изменить сотрудника. " +
                            "\n\t Запись удалена или не найдена");
        }
        
        // Меняем поле на удаленное, а запись перезаписываем
        worker.Deleted = true;
        await _workerService.UpdateAsync(id, worker);

        return NoContent();
    }
}