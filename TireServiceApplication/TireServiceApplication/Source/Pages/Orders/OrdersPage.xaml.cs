using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Orders;

public partial class OrdersPage : ContentPage
{
    public OrdersPage()
    {
        InitializeComponent();
        InitializePicker();
    }

    protected override void OnAppearing()
    {
        RefreshData();
        base.OnAppearing();
    }
    
    // Инициализация выбора статуса заказа для фильтра
    private void InitializePicker()
    {
        var orderStatuses = OrderStatusModel.OrderStatuses;
        var newOrderStatus = new OrderStatus { Title = "Все", Key = null, Color = new Color(0,0,0)};
        orderStatuses.Add(newOrderStatus);
        StatusPicker.ItemsSource = orderStatuses;
        StatusPicker.ItemDisplayBinding = new Binding("Title");
        StatusPicker.Title = "Статус";
        StatusPicker.FontSize = 18;
        StatusPicker.VerticalOptions = LayoutOptions.Center;
        StatusPicker.SelectedItem = newOrderStatus;
        StatusPicker.SelectedIndexChanged += (sender, args) => RefreshData();
    }

    // Обновление данных с учетом фильтра
    private async void RefreshData()
    {
        ListView.ItemsSource = null;
        var selectedStatus = (OrderStatus)StatusPicker.SelectedItem;
        StatusPicker.TextColor = selectedStatus.Color;
        List<Order> result;
        if (selectedStatus.Key != null)
            result = await OrderModel.GetOrdersByStatus(selectedStatus);
        else
            result = await OrderModel.GetOrders();
        ListView.ItemsSource = result;
    }

    // Переход на страницу просмотра заказа
    private async void ListView_OnItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null) return;
        var order = (Order)e.SelectedItem;
        await Navigation.PushAsync(new ViewOrderPage(order));
        ListView.SelectedItem = null;
    }
    
    // Переход на страницу добавления заказа
    private async void AddButton_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddOrderPage());
    }
}