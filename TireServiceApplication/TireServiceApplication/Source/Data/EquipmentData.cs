using System.Net;
using Newtonsoft.Json;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Data;

public class EquipmentData
{
    private const string GetAllEquipmentUrl = "getAllByBranch";
    private const string EquipmentUrl = "Equipment";
    
    // Метод для получения всего оборудования из БД
    public static async Task<List<Equipment>?> GetEquipment()
    {
        try
        {
            var result = await ApiClient.Get($"{EquipmentUrl}/{GetAllEquipmentUrl}");
            var content = JsonConvert.DeserializeObject<List<Equipment>>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return new List<Equipment>();
            throw;
        }
    }
    
    // Метод для добавления оборудования в БД
    public static async Task<Equipment?> AddEquipment(Equipment equipment)
    {
        try
        {
            var result = await ApiClient.Post($"{EquipmentUrl}", equipment);
            var content = JsonConvert.DeserializeObject<Equipment>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }
    
    // Метод для изменения оборудования в БД
    public static async Task<Equipment?> UpdateEquipment(Equipment equipment)
    {
        try
        {
            var result = await ApiClient.Put($"{EquipmentUrl}", equipment);
            var content = JsonConvert.DeserializeObject<Equipment>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }
    
    // Метод для удаления оборудования из БД
    public static async Task<bool> DeleteEquipment(string id)
    {
        try
        {
            var result = await ApiClient.Delete($"{EquipmentUrl + "/" + id}");
            return result.StatusCode == HttpStatusCode.NoContent;
        }
        catch (Exception e)
        {
            return false;
            throw;
        }
    }

}