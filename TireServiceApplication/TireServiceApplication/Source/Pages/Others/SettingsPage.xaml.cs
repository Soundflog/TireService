using TireServiceApplication.Source.Data;
using TireServiceApplication.Source.Pages.Branches;
using TireServiceApplication.Source.Pages.Equipments;
using TireServiceApplication.Source.Pages.Reports;
using TireServiceApplication.Source.Pages.Users;

namespace TireServiceApplication.Source.Pages.Others;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    // Скрыть кнопки от пользователей, которые не являются администраторами
    protected override void OnAppearing()
    {
        if (Storage.GetRole() != "admin")
        {
            ManagersButton.IsVisible = false;
            BranchesButton.IsVisible = false;
            ReportsButton.IsVisible = false;
        }
        else
        {
            ManagersButton.IsVisible = true;
            BranchesButton.IsVisible = true;
            ReportsButton.IsVisible = true;
        }

        base.OnAppearing();
    }

    // Выйти из аккаунта
    private void LogOutButton_OnClicked(object? sender, EventArgs e)
    {
        Storage.RemoveToken();
        Storage.RemoveRole();
        App.SetAuthenticationPage();
    }

    // Перейти на страницу оборудования
    private async void EquipmentsButton_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new EquipmentPage());
    }

    // Перейти на страницу филиалов
    private async void BranchesButton_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new BranchesPage());
    }

    // Перейти на страницу пользователей
    private async void ManagersButton_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new UsersPage());
    }

    // Перейти на страниу отчетов
    private async void ReportsButton_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new ReportsPage());
    }
}