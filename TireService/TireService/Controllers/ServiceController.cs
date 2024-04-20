using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TireService.Models;
using TireService.Services;

namespace TireService.Controllers;

// Котроллер услуга

[ApiController]
[Route("api/[controller]")]
public class ServiceController : ControllerBase
{
    private readonly ServiceService _serviceService;
    private readonly BranchService _branchService;
    private readonly EquipmentService _equipmentService;
    private readonly UserService _userService;

    public ServiceController(ServiceService serviceService, BranchService branchService,
        EquipmentService equipmentService, UserService userService)
    {
        _serviceService = serviceService;
        _branchService = branchService;
        _equipmentService = equipmentService;
        _userService = userService;
    }
    

    [HttpGet, Authorize(Roles = "admin")]
    public async Task<List<Service>> GetAllWithEquipment()
    {
        return await _serviceService.GetAllAsync();
    }

    // Фильтр по филлиалу
    [HttpGet, Authorize(Roles = "admin, manager")]
    [Route("getAllByBranch")]
    public async Task<List<Service>?> GetAllByBranch()
    {
        var username = User.Identity!.Name;
        if (username == null) return null;
        
        var user = await _userService.GetAsync(username);
        
        if (user?.BranchId?.Id == null) return null;

        var temp = await _serviceService.GetByBranch(user.BranchId.Id);
        foreach (var equipment in temp!.SelectMany(service => service.EquipmentId!))
        {
            equipment!.EstimatedValue = null;
        }
        return temp;
    }
    
    [HttpPost, Authorize(Roles = "admin, manager")]
    public async Task<IActionResult> Post(Service newService)
    {
        if (newService.Deleted == true 
            || newService.Cost < 0 
            || newService.DurationInMinutes is null 
            || newService.Title is null)
            return NotFound("Невозможно создать услугу \n\t Запись удалена " +
                            "\n\t Стоимость меньше 0 \n\t Продолжительность = null" +
                            "\n\t Название услуги null \n\t Id филиала не указано");
        
        var username = User.Identity!.Name;
        if (username == null) return NotFound("No Authorization");
        
        var user = await _userService.GetAsync(username);
        
        if (user?.BranchId?.Id == null) return NotFound("BranchId = null");
        
        var branch = await _branchService.GetAsync(user.BranchId.Id);
        
        var equipments = await _equipmentService.GetByBranch(user.BranchId.Id);
        newService.BranchId = branch;
            
        /*
             Перебираем оборудования, которые относятся к данному филиалу.
             Сравниваем все оборудования по id с бд
             Если да, то записываем это оборудование.
             Если нет, то null 
            */
        if (equipments.Any())
        {
            foreach (var eq in equipments.Where(_ => newService.EquipmentId != null))
            {
                if (newService.EquipmentId == null) continue;
                for (var i = 0; i < newService.EquipmentId.Length; i++)
                {
                    if (newService.EquipmentId[i]?.Deleted != true
                        && newService.EquipmentId[i]?.Id == eq.Id)
                    {
                        newService.EquipmentId[i] = eq;
                    }
                }
            }
        }

        await _serviceService.CreateAsync(newService);
        return CreatedAtAction(nameof(GetAllByBranch), new { id = newService.Id }, newService);

    }

    [HttpPut, Authorize(Roles = "admin, manager")]
    public async Task<IActionResult> Update(Service updatedService)
    {
        if (updatedService.Id == null)
        {
            return NotFound("Не указан ID");
        }
        string id = updatedService.Id;
        var service = await _serviceService.GetAsync(id);

        if (service is null || service.Deleted == true || updatedService.Cost < 0)
        {
            return NotFound("Невозможно изменить услугу. " +
                            "\n\t Запись удалена или не найдена " +
                            "\n\t Стоимость меньше 0");
        }

        updatedService.Id = service.Id;
        /*
          Проверка поля Филиала на заполненость,
         если нет, то оставляем значение таким какое было,
         если да, то проверяем есть ли такой филиал по id и заполняем
         */
        var username = User.Identity!.Name;
        if (username == null) return NotFound("No Authorization");
        
        var user = await _userService.GetAsync(username);
        
        if (user?.BranchId?.Id == null) return NotFound("BranchId = null");
        
        var branch = await _branchService.GetAsync(user.BranchId.Id);
        updatedService.BranchId = branch;
        
        /*
          Проверка поля Оборудование на заполненость и не удалена ли запись,
         если нет, то оставляем значение таким какое было,
         если да, то находим по id оборудование и записываем 
         */
        if (updatedService.EquipmentId != null)
        {
            var length = updatedService.EquipmentId!.Length;
            Equipment?[] equipments = new Equipment?[length];
            for (var i = 0; i < length; i++)
            {
                var idEquipment = updatedService.EquipmentId[i]!.Id;
                if (updatedService.EquipmentId[i]!.Deleted == true
                    || idEquipment == null) continue;
                var eq = await _equipmentService.GetAsync(idEquipment);
                equipments[i] = eq;
            }
            updatedService.EquipmentId = equipments;
        }
        else
            updatedService.EquipmentId = service.EquipmentId;

        await _serviceService.UpdateAsync(id, updatedService);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}"), Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var service = await _serviceService.GetAsync(id);

        if (service is null || service.Deleted == true)
        {
            return NotFound("Невозможно удалить услугу. " +
                            "\n\t Запись удалена или не найдена");
        }
        // Меняем поле deleted на false, а запись перезаписываем
        service.Deleted = true;
        await _serviceService.UpdateAsync(id, service);

        return NoContent();
    }
}