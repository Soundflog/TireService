using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Models;

public class OrderStatusModel
{
    // Список статусов заказов "Plan" "InProcess" "Cancel" "Success" "Unsuccess"
    public static List<OrderStatus> OrderStatuses { get; } = new List<OrderStatus>()
    {
        new OrderStatus() {Title = "Запланирован", Key = "Plan", Color = Color.FromHex("#FFC107")},
        new OrderStatus() {Title = "В процессе", Key = "InProcess", Color = Color.FromHex("#FF9800")},
        new OrderStatus() {Title = "Выполнен", Key = "Success", Color = Color.FromHex("#4CAF50")},
        new OrderStatus() {Title = "Не выполнен", Key = "Unsuccess", Color = Color.FromHex("#F44336")},
        new OrderStatus() {Title = "Отменен", Key = "Cancel", Color = Color.FromHex("#9E9E9E")}
    };

    // Поиск по ключу
    public static OrderStatus OrderStatusByKey(string key)
    {
        // Вернуть статус заказа по ключу, если такого нет вернуть Title = "", Key = "", Color = Color.FromHex("#000000")
        return OrderStatuses.Find(x => x.Key == key) ?? new OrderStatus()
            { Title = "", Key = "", Color = Color.FromHex("#000000") };
    }
    
}