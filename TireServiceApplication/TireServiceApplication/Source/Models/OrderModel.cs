using System.Globalization;
using TireServiceApplication.Source.Data;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Models;

public class OrderModel
{
    private static List<Order> _orders = new List<Order>();
    
    // Метод для получения всех заказов
    public static async Task<List<Order>?> GetOrders()
    {
        var orders = await OrderData.GetOrders();
        if (orders != null)
        {
            _orders = orders.OrderByDescending(x => x.StartDate).ToList();
            ObjectForListView(_orders);
        }
        return _orders;
    }
    
    // Метод для получения всех заказов по статусу
    public static async Task<List<Order>?> GetOrdersByStatus(OrderStatus status)
    {
        var orders = await OrderData.GetOrders();
        if (orders != null)
        {
            _orders = orders.Where(x => x.Status == status.Key).ToList();
            _orders = _orders.OrderByDescending(x => x.StartDate).ToList();
            ObjectForListView(_orders);
        }
        return _orders;
    }
    
    // Метод для добавления заказа
    public static async Task<bool> AddOrder(Order order)
    {
        var newOrder = await OrderData.AddOrder(order);
        if (newOrder != null) _orders.Add(newOrder);
        return newOrder != null;
    }
    
    // Метод для изменения заказа
    public static async Task<bool> UpdateOrder(Order order)
    {
        var newOrder = await OrderData.UpdateOrder(order);
        if (newOrder != null)
        {
            var index = _orders.FindIndex(x => x.Id == order.Id);
            _orders[index] = newOrder;
        }
    
        return newOrder != null;
    }
    
    // Метод для удаления заказа
    public static async Task<bool> DeleteOrder(Order order)
    {
        if (order.Id == null) return false;
        var result = await OrderData.DeleteOrder(order.Id);
        if (result)
        {
            var index = _orders.FindIndex(x => x.Id == order.Id);
            _orders.RemoveAt(index);
        }
    
        return result;
    }
    
    // Метод для отображения информации в ListView
    private static void ObjectForListView(List<Order> orders)
    {
        foreach (var order in orders)
        {
            var orderStatus = OrderStatusModel.OrderStatusByKey(order.Status);
            order.TitleView = $"Заказ №{order.StartDate?.ToString("dd.MM.yyyy.HH.mm").Replace(".","")}";
            order.CostView = $"Стоимость: {order.Cost} руб";
            order.StatusView = $"{orderStatus.Title}";
            order.StatusColorView = orderStatus.Color;
        }
    }

    // Метод для изменения статуса заказа
    public static async Task<bool> ChangeStatus(Order order, string status)
    {
        var newOrder = order.Id != null && await OrderData.ChangeStatus(order.Id,status);
        return newOrder;
    }
    
    // Метод для получения отчета по заказам
    public static async Task<Response> GetReportOrder(string email, DateTime startDate, DateTime endDate)
    {
        var orders = await OrderData.GetOrderReport(email, startDate.ToString(CultureInfo.InvariantCulture), endDate.ToString(CultureInfo.InvariantCulture));
        return orders;
    }
    
    // Метод для получения отчета по сотрудникам
    public static async Task<Response> GetReportPersonal(string email, DateTime startDate, DateTime endDate)
    {
        var orders = await OrderData.GetPersonalReport(email, startDate.ToString(CultureInfo.InvariantCulture), endDate.ToString(CultureInfo.InvariantCulture));
        return orders;
    }
    
    // Метод для получения отчета по заказам всех филиалов
    public static async Task<Response> GetAllReportOrder(string email, DateTime startDate, DateTime endDate)
    {
        var orders = await OrderData.GetAllOrderReport(email, startDate.ToString(CultureInfo.InvariantCulture), endDate.ToString(CultureInfo.InvariantCulture));
        return orders;
    }
    
    // Метод для получения отчета по сотрудникам всех филиалов
    public static async Task<Response> GetAllReportPersonal(string email, DateTime startDate, DateTime endDate)
    {
        var orders = await OrderData.GetAllPersonalReport(email, startDate.ToString(CultureInfo.InvariantCulture), endDate.ToString(CultureInfo.InvariantCulture));
        return orders;
    }
}