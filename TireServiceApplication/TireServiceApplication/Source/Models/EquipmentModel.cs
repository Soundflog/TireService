using TireServiceApplication.Source.Data;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Models;

public class EquipmentModel
{
    private static List<Equipment> _equipments = new List<Equipment>();

    // Метод для получения всего оборудования
    public static async Task<List<Equipment>?> GetEquipments()
    {
        var equipments = await EquipmentData.GetEquipment();
        if (equipments != null) _equipments = equipments;
        return _equipments;
    }

    // Метод для добавления оборудования
    public static async Task<bool> AddEquipment(Equipment equipment)
    {
        var newEquipment = await EquipmentData.AddEquipment(equipment);
        if (newEquipment != null) _equipments.Add(newEquipment);
        return newEquipment != null;
    }

    // Метод для изменения оборудования
    public static async Task<bool> UpdateEquipment(Equipment equipment)
    {
        var newEquipment = await EquipmentData.UpdateEquipment(equipment);
        if (newEquipment != null)
        {
            var index = _equipments.FindIndex(x => x.Id == equipment.Id);
            _equipments[index] = newEquipment;
        }

        return newEquipment != null;
    }

    // Метод для удаления оборудования
    public static async Task<bool> DeleteEquipment(Equipment equipment)
    {
        if (equipment.Id == null) return false;
        var result = await EquipmentData.DeleteEquipment(equipment.Id);
        if (result)
        {
            var index = _equipments.FindIndex(x => x.Id == equipment.Id);
            _equipments.RemoveAt(index);
        }

        return result;
    }

}