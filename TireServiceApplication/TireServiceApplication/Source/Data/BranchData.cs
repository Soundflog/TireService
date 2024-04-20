using System.Net;
using Newtonsoft.Json;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Data;

public class BranchData
{
    private const string BranchesUrl = "Branch";
    
    // Метод для получения всех филиалов из БД
    public static async Task<List<Branch>?> GetBranches()
    {
        try
        {
            var result = await ApiClient.Get($"{BranchesUrl}");
            var content = JsonConvert.DeserializeObject<List<Branch>>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return new List<Branch>();
            throw;
        }
    }
    
    // Метод для добавления филиала в БД
    public static async Task<Branch?> AddBranch(Branch branch)
    {
        try
        {
            var result = await ApiClient.Post($"{BranchesUrl}", branch);
            var content = JsonConvert.DeserializeObject<Branch>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }

    // Метод для изменения филиала в БД
    public static async Task<Branch?> UpdateBranch(Branch branch)
    {
        try
        {
            var result = await ApiClient.Put($"{BranchesUrl}", branch);
            var content = JsonConvert.DeserializeObject<Branch>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }

    // Метод для удаления филиала из БД
    public static async Task<bool> DeleteBranch(string id)
    {
        try
        {
            var result = await ApiClient.Delete($"{BranchesUrl + "/" + id}");
            return result.StatusCode == HttpStatusCode.NoContent;
        }
        catch (Exception e)
        {
            return false;
            throw;
        }
    }
    
    
}