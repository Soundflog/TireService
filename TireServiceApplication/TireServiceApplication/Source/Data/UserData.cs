using System.Net;
using Newtonsoft.Json;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Data;

public class UserData
{
    private const string GetUserUrl = "getAllExists";
    private const string GetOneByBranchUrl = "getOneByBranch";
    private const string GetYourselfUrl = "getYourself";
    private const string UserUrl = "User";
    
    // Метод для получения всех пользователей из БД
    public static async Task<List<User>?> GetUsers()
    {
        try
        {
            var result = await ApiClient.Get($"{UserUrl}/{GetUserUrl}");
            var content = JsonConvert.DeserializeObject<List<User>>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return new List<User>();
            throw;
        }
    }
    
    // Метод для добавления пользователя в БД
    public static async Task<User?> AddUser(User user)
    {
        try
        {
            var result = await ApiClient.Post($"{UserUrl}", user);
            var content = JsonConvert.DeserializeObject<User>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }
    
    // Метод для изменения пользователя в БД
    public static async Task<User?> UpdateUser(User user)
    {
        try
        {
            var result = await ApiClient.Put($"{UserUrl}", user);
            var content = JsonConvert.DeserializeObject<User>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }
    
    // Метод для удаления пользователя из БД
    public static async Task<bool> DeleteUser(string id)
    {
        try
        {
            var result = await ApiClient.Delete($"{UserUrl + "/" + id}");
            return result.StatusCode == HttpStatusCode.NoContent;
        }
        catch (Exception e)
        {
            return false;
            throw;
        }
    }
    
    // Метод для получения своих данных из БД (по token)
    public static async Task<User?> GetUser()
    {
        try
        {
            var result = await ApiClient.Get($"{UserUrl}/{GetYourselfUrl}");
            if (result.StatusCode == HttpStatusCode.Unauthorized) return null;
            var content = JsonConvert.DeserializeObject<User>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }
    
    // Метод для получения пользователя по Id из БД
    public static async Task<User?> GetUserById(string id)
    {
        try
        {
            var result = await ApiClient.Get($"{UserUrl}/{GetOneByBranchUrl}?id={id}");
            var content = JsonConvert.DeserializeObject<User>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }
}