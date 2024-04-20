using TireServiceApplication.Source.Data;
using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Orders;

public partial class ViewOrderPage : ContentPage
{
    private Order _order;
    private List<OrderStatus> _statusList;
    public ViewOrderPage(Order order)
    {
        InitializeComponent();
        _order = order;
        _statusList = OrderStatusModel.OrderStatuses;
        Init();
        
        // Скрыть кнопку "Удалить" от пользователей, которые не являются администраторами
        if (Storage.GetRole() != "admin")
        {
            DeleteButton.IsVisible = false;
        }
    }
    
    private void Init()
    {
        Title = _order.TitleView;
        InitStatusPicker();
        
        // Вывод общей цены заказа
        CostLabel.Text = _order.CostView;

        // Вывод даты и времени начала и завершения заказа
        StartWorkLabel.Text = $"{_order.StartDate?.ToString("dd.MM.yyyy HH:mm")}";
        EndWorkLabel.Text = $"{_order.EndDate?.ToString("dd.MM.yyyy HH:mm")}";

        // Вывод сотрудника, который будет работать над заказом
        WorkerLabel.Text = $"{_order.WorkerId?.FirstName} {_order.WorkerId?.LastName}";
        
        // Вывод клиента, который оформил заказ
        ClientLabel.Text = $"{_order.ClientId?.FirstName} {_order.ClientId?.LastName}";
        
        // Вывод услуг списком
        ServiceStackLayout.Children.Clear();
        if (_order.ServiceId != null)
        {
            for (var i = 0; i < _order.ServiceId.Length; i++)
            {
                var service = _order.ServiceId[i];
                var label = new Label();
                label.Text = $"{i+1}. {service?.Title}";
                label.FontSize = 18;
                label.HorizontalOptions = LayoutOptions.Start;
                label.VerticalOptions = LayoutOptions.Center;
                ServiceStackLayout.Children.Add(label);
            }
        }
    }
    
    // Инициализация выбора статуса заказа
    private void InitStatusPicker()
    {
        StatusPicker.ItemsSource = _statusList;
        StatusPicker.ItemDisplayBinding = new Binding("Title");
        StatusPicker.FontSize = 18;
        StatusPicker.SelectedItem = _statusList.FirstOrDefault(x => x.Key == _order.Status);
        StatusPicker.SelectedIndexChanged += (sender, args) => UpdateStatus();
    }
    
    // Удаление заказа
    private async void DeleteButton_OnClicked(object? sender, EventArgs e)
    {
        var alert = await DisplayAlert("Подтверждение", "Вы действительно хотите удалить заказ?", "Да", "Отмена");
        if (!alert) return;
        var result = await OrderModel.DeleteOrder(_order);
        if (result == false) await DisplayAlert("Внимание", "Не удалось удалить заказ.", "Ок");
        else await Navigation.PopAsync();
    }

    // Обновление статуса заказа
    private async void UpdateStatus()
    {
        if (StatusPicker.SelectedItem is OrderStatus status)
        {
            _order.Status = status.Key;
            _order.StatusView = status.Title;
            var result = await OrderModel.ChangeStatus(_order, _order.Status);
            if (result == false) await DisplayAlert("Внимание", "Не удалось изменить статус заказа.", "Ок");
        }
    }
}