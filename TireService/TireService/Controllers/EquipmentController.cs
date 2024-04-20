using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TireService.Models;
using TireService.Services;

namespace TireService.Controllers;

// Котроллер Оборудования

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly EquipmentService _equipmentService;
    private readonly BranchService _branchService;
    private readonly UserService _userService;

    public EquipmentController(EquipmentService equipmentService, BranchService branchService, UserService userService)
    {
        _equipmentService = equipmentService;
        _branchService = branchService;
        _userService = userService;
    }

    // Запрос GetAll - выдаёт всех оборудований из бд
    [HttpGet, Authorize(Roles = "admin")]
    public async Task<List<Equipment>> GetAll() =>
        await _equipmentService.GetAllAsync();

    // Фильтр по филлиалу
    [HttpGet, Authorize(Roles = "admin, manager")]
    [Route("getAllByBranch")]
    public async Task<List<Equipment>?> GetAllByBranch()
    {
        var username = User.Identity!.Name;
        if (username == null) return null;
        
        var user = await _userService.GetAsync(username);
        
        if (user?.BranchId?.Id == null) return null;

        var temp = await _equipmentService.GetByBranch(user.BranchId.Id);
        foreach (var equipment in temp)
        {
            equipment.EstimatedValue = null;
        }
        
        return temp;
    }

    /* оценночная стоимость и амортизация по месяцам (массив. Тут будет максимум
    12 значений, это оценночная стоимость на каждый месяц до конца года и
    стоимость амортизации за этот месяц, это всё будет считаться по формулам
    ниже):
        * месяц (номер месяца, в который куплен);
        * оценночная стоимость в рублях (для месяца с покупкой устанавливается
    значение равное стоимости оборудования при покупке, далее по формуле:
    оцен. стоимость предыдущего месяца - амортизация за пред. месяц);
        * амортизация в рублях (оцен. стоимость, которая посчитана для этого
    месяца/срок полезного использования/12) ((линейный способ подсчета
        амортизации)).*/

    private static Equipment Calculate(Equipment equipment, Equipment? similarEquip)
    {
        // Вычисление амортизации
        if (similarEquip != null &&
            (equipment.UsefulLifeDate is null
             || equipment.PurchaseDate is null
             || equipment.BuildDate is null
             || equipment.GuaranteeDate is null))
        {
            equipment.UsefulLifeDate = similarEquip.UsefulLifeDate;
            equipment.PurchaseDate = similarEquip.PurchaseDate;
            equipment.BuildDate = similarEquip.BuildDate;
            equipment.GuaranteeDate = similarEquip.GuaranteeDate;
        }

        var spiAllYear = equipment.UsefulLifeDate!.Value;
        var purchaseMonth = equipment.PurchaseDate!.Value.Month;
        List<Estimated> newEstimateds = new List<Estimated>(12);
        for (int i = 0; i < 12; i++)
        {
            // Проверка номера месяца (не 0)
            // Если номер месяца > 12 тогда -12 + i, иначе +i
            Estimated estimated = new Estimated();
            estimated.Month = DateTime.Now;
            var addMonths = estimated.Month.Value.AddMonths(i);
            estimated.Month = addMonths;

            // Оценочная стоимость 1 месяц = стоимости оборудования, остальные по оцен/СПИ/12
            if (i == 0)
                estimated.EstimatedCost = equipment.Cost;
            else
                estimated.EstimatedCost =
                    newEstimateds[i - 1].EstimatedCost - newEstimateds[i - 1].Amortization;

            estimated.Amortization = estimated.EstimatedCost / spiAllYear / 12;
            newEstimateds.Add(estimated);
        }

        equipment.EstimatedValue = newEstimateds;
        return equipment;
    }

    [HttpPost, Authorize(Roles = "admin, manager")]
    public async Task<IActionResult> Post(Equipment newEquipment)
    {
        if (newEquipment.Deleted == true || newEquipment.Id is not null
            || newEquipment.Cost < 0 
            || newEquipment.Title is null 
            || newEquipment.BuildDate is null
            || newEquipment.GuaranteeDate <= DateTime.Now
            || newEquipment.PurchaseDate is null )
            return NotFound("Невозможно создать оборудование\n\t Стоимость < 0 " +
                            "\n\t Неуказаны даты постройки, гарантии, покупки");
        
        var username = User.Identity!.Name;
        if (username == null) return NotFound("username not found");
        
        var user = await _userService.GetAsync(username);
        
        if (user?.BranchId?.Id == null) return NotFound("User does not have id");

        var branch = await _branchService.GetAsync(user.BranchId.Id);
        
        newEquipment.BranchId = branch;
        // Расчитать амортизацию
        var newEquipmentCalculated = Calculate(newEquipment, null);
        await _equipmentService.CreateAsync(newEquipmentCalculated);
        
        return CreatedAtAction(nameof(GetAll),
            new { id = newEquipment.Id }, newEquipment);
    }

    [HttpPut, Authorize(Roles = "admin")]
    public async Task<IActionResult> Update( Equipment updatedEquipment)
    {
        var id = updatedEquipment.Id;
        if (id == null) return NotFound("WRONG: Id не указано");

        if (updatedEquipment.Deleted == true 
            || updatedEquipment.Cost < 0 
            || updatedEquipment.Title is null
            || updatedEquipment.BuildDate is null
            || updatedEquipment.PurchaseDate is null
            || updatedEquipment.GuaranteeDate <= updatedEquipment.PurchaseDate
            || updatedEquipment.UsefulLifeDate is null)
            return NotFound("Невозможно изменить оборудование: \n Возможные ошибки " +
                            "\n\t Создать удаленую запись \n\t Id должно быть пустым " +
                            "\n\t Обязательные поля: название, даты постройки, гарантии, покупки, СПИ");
        
        var equipment = await _equipmentService.GetAsync(id);

        if (equipment is null || equipment.Deleted == true)
        {
            return NotFound("Невозможно изменить оборудование." +
                            "\n\t Запись удалена или не найдена");
        }
        
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

        /*
          Проверка поля Оборудование на изменения цены,
         если нет, то оставляем значение таким какое было,
         если да, то пересчитываем оценочную стоимость и амортизацию
         */
        updatedEquipment.Id = equipment.Id;
        updatedEquipment.BranchId = branch;
        
        if (updatedEquipment.Cost != null || !updatedEquipment.Cost!.Value.Equals(equipment.Cost!.Value))
        {
            if (updatedEquipment.Id != null)
            {
                var similarEq = await _equipmentService.GetAsync(updatedEquipment.Id);
                updatedEquipment = Calculate(updatedEquipment, similarEq);
            }
            else
                return NotFound("Не удалось пересчитать амортизацию");
        }

        await _equipmentService.UpdateAsync(id, updatedEquipment);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}"), Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var equipment = await _equipmentService.GetAsync(id);

        if (equipment is null || equipment.Deleted == true)
        {
            return NotFound("Невозможно изменить оборудование." +
                            "\n\t Запись удалена или не найдена");
        }

        // Меняем поле на удаленное, а запись перезаписываем
        equipment.Deleted = true;
        await _equipmentService.UpdateAsync(id, equipment);

        return NoContent();
    }
}