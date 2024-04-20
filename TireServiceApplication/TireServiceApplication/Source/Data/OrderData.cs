using System.Net;
using Newtonsoft.Json;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Data;

public class OrderData
{
    private const string GetAllOrdersUrl = "getAllByBranch";
    private const string ChangeStatusUrl = "changeStatus";
    private const string OrdersUrl = "Order";
    private const string GetReportOrderUrl = "getReportOrder";
    private const string GetAllReportOrderUrl = "getReportAllOrders";
    private const string GetReportPersonalUrl = "getReportPersonal";
    private const string GetAllReportPersonalUrl = "getReportAllPersonal";
    
    // Метод для получения всех заказов из БД
    public static async Task<List<Order>?> GetOrders()
    {
        try
        {
            var result = await ApiClient.Get($"{OrdersUrl}/{GetAllOrdersUrl}");
            var content = JsonConvert.DeserializeObject<List<Order>>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return new List<Order>();
            throw;
        }
    }
    
    // Метод для добавления заказа в БД
    public static async Task<Order?> AddOrder(Order order)
    {
        try
        {
            var result = await ApiClient.Post($"{OrdersUrl}", order);
            var content = JsonConvert.DeserializeObject<Order>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }
    
    // Метод для изменения заказа в БД
    public static async Task<Order?> UpdateOrder(Order order)
    {
        try
        {
            var result = await ApiClient.Put($"{OrdersUrl}", order);
            var content = JsonConvert.DeserializeObject<Order>(result.Content);
            return content;
        }
        catch (Exception e)
        {
            return null;
            throw;
        }
    }
    
    // Метод для удаления заказа из БД
    public static async Task<bool> DeleteOrder(string id)
    {
        try
        {
            var result = await ApiClient.Delete($"{OrdersUrl + "/" + id}");
            return result.StatusCode == HttpStatusCode.NoContent;
        }
        catch (Exception e)
        {
            return false;
            throw;
        }
    }
    
    // Метод для изменения статуса заказа
    public static async Task<bool> ChangeStatus(string id, string status)
    {
        try
        {
            var result = await ApiClient.Post($"{OrdersUrl}/{ChangeStatusUrl}?id={id}&status={status}", null);
            return result.StatusCode == HttpStatusCode.NoContent;
        }
        catch (Exception e)
        {
            return false;
            throw;
        }
    }
    
    // Метод для получения отчета по заказам
    public static async Task<Response> GetOrderReport(string email, string startData, string endData)
    {
        try
        {
            var result = await ApiClient.Get($"{OrdersUrl}/{GetReportOrderUrl}?email={email}&startDate={startData}&endDate={endData}");
            return result;
        }
        catch (Exception e)
        {
            return new Response(HttpStatusCode.NoContent, "");
            throw;
        }
    }
    
    // Метод для получения отчета по персоналу
    public static async Task<Response> GetPersonalReport(string email, string startData, string endData)
    {
        try
        {
            var result = await ApiClient.Get($"{OrdersUrl}/{GetReportPersonalUrl}?email={email}&startDate={startData}&endDate={endData}");
            return result;
        }
        catch (Exception e)
        {
            return new Response(HttpStatusCode.NoContent, "");
            throw;
        }
    }
    
    // Метод для получения отчета по заказам всех филиалов
    public static async Task<Response> GetAllOrderReport(string email, string startData, string endData)
    {
        try
        {
            var result = await ApiClient.Get($"{OrdersUrl}/{GetAllReportOrderUrl}?email={email}&startDate={startData}&endDate={endData}");
            return result;
        }
        catch (Exception e)
        {
            return new Response(HttpStatusCode.NoContent, "");
            throw;
        }
    }
    
    // Метод для получения отчета по персоналу всех филиалов
    public static async Task<Response> GetAllPersonalReport(string email, string startData, string endData)
    {
        try
        {
            var result = await ApiClient.Get($"{OrdersUrl}/{GetAllReportPersonalUrl}?email={email}&startDate={startData}&endDate={endData}");
            return result;
        }
        catch (Exception e)
        {
            return new Response(HttpStatusCode.NoContent, "");
            throw;
        }
    }
}