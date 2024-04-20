using System.Net;
using Newtonsoft.Json;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Data;

public class WorkerData
{
    private const string GetAllWorkersUrl = "getAllByBranch";
    private const string WorkersUrl = "Worker";
    
    // Метод для получения всех работников из БД
    public static async Task<List<Worker>?> GetWorkers()
    {
        try
        {
            var result = await ApiClient.Get($"{WorkersUrl}/{GetAllWorkersUrl}");
            var content = JsonConvert.DeserializeObject<List<Worker>>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return new List<Worker>();
            throw;
        }
    }
    
    // Метод для добавления работника в БД
    public static async Task<Worker?> AddWorker(Worker worker)
    {
        try
        {
            var result = await ApiClient.Post($"{WorkersUrl}", worker);
            var content = JsonConvert.DeserializeObject<Worker>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }
    
    // Метод для изменения работника в БД
    public static async Task<Worker?> UpdateWorker(Worker worker)
    {
        try
        {
            var result = await ApiClient.Put($"{WorkersUrl}", worker);
            var content = JsonConvert.DeserializeObject<Worker>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }
    
    // Метод для удаления работника из БД
    public static async Task<bool> DeleteWorker(string id)
    {
        try
        {
            var result = await ApiClient.Delete($"{WorkersUrl + "/" + id}");
            return result.StatusCode == HttpStatusCode.NoContent;
        }
        catch (Exception e)
        {
            return false;
            throw;
        }
    }
}