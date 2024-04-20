using System.Net;
using Newtonsoft.Json;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Data;

public class ClientData
{
    private const string ClientUrl = "Client";
    
    // Метод для получения всех клиентов из БД
    public static async Task<List<Client>?> GetClients()
    {
        try
        {
            var result = await ApiClient.Get($"{ClientUrl}");
            var content = JsonConvert.DeserializeObject<List<Client>>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return new List<Client>();
            throw;
        }
    }
    
    // Метод для добавления клиента в БД
    public static async Task<Client?> AddClient(Client client)
    {
        try
        {
            var result = await ApiClient.Post($"{ClientUrl}", client);
            var content = JsonConvert.DeserializeObject<Client>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }
    
    // Метод для изменения клиента в БД
    public static async Task<Client?> UpdateClient(Client client)
    {
        try
        {
            var result = await ApiClient.Put($"{ClientUrl}", client);
            var content = JsonConvert.DeserializeObject<Client>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }
    
    // Метод для удаления клиента из БД
    public static async Task<bool> DeleteClient(string id)
    {
        try
        {
            var result = await ApiClient.Delete($"{ClientUrl}/{id}");
            return result.StatusCode == HttpStatusCode.NoContent;
        }
        catch (Exception e)
        {
            return false;
            throw;
        }
    }
}