using System.Net;
using Newtonsoft.Json;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Data;

public class ServiceData
{
    private const string GetAllServicesUrl = "getAllByBranch";
    private const string ServicesUrl = "Service";
    
    // Метод для получения всех услуг из БД
    public static async Task<List<Service>?> GetServices()
    {
        try
        {
            var result = await ApiClient.Get($"{ServicesUrl}/{GetAllServicesUrl}");
            var content = JsonConvert.DeserializeObject<List<Service>>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return new List<Service>();
            throw;
        }
    }
    
    // Метод для добавления услуги в БД
    public static async Task<Service?> AddService(Service service)
    {
        try
        {
            var result = await ApiClient.Post($"{ServicesUrl}", service);
            var content = JsonConvert.DeserializeObject<Service>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }
    
    // Метод для изменения услуги в БД
    public static async Task<Service?> UpdateService(Service service)
    {
        try
        {
            var result = await ApiClient.Put($"{ServicesUrl}", service);
            var content = JsonConvert.DeserializeObject<Service>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }
    
    // Метод для удаления услуги из БД
    public static async Task<bool> DeleteService(string id)
    {
        try
        {
            var result = await ApiClient.Delete($"{ServicesUrl + "/" + id}");
            return result.StatusCode == HttpStatusCode.NoContent;
        }
        catch (Exception e)
        {
            return false;
            throw;
        }
    }
}