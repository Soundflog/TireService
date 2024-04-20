using TireServiceApplication.Source.Data;
using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Equipments;

public partial class EquipmentPage : ContentPage
{
    public EquipmentPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        RefreshData();
        base.OnAppearing();
    }

    private async void RefreshData()
    {
        ListView.ItemsSource = null;
        var result = await EquipmentModel.GetEquipments();
        ListView.ItemsSource = result;
    }

    // Переход на страницу добавления оборудования
    private async void AddButton_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddEquipmentPage());
    }

    // Переход на страницу изменения оборудования
    private async void ListView_OnItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        // Если пользователь не является админом - блокировать действие
        if (Storage.GetRole() != "admin")
        {
            ListView.SelectedItem = null;
            return;
        }
        if (e.SelectedItem == null) return;
        var equipment = (Equipment)e.SelectedItem;
        await Navigation.PushAsync(new SettingEquipmentPage(equipment));
        ListView.SelectedItem = null;
    }
}