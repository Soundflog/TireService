using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Users;

public partial class UsersPage : ContentPage
{
    public UsersPage()
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
        var result = await UserModel.GetUsers();
        ListView.ItemsSource = result;
    }
    
    // Переход на страницу добавления пользователя
    private async void AddButton_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddUserPage());
    }
    
    // Переход на страницу изменения пользователя
    private async void ListView_OnItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null) return;
        var user = (User)e.SelectedItem;
        await Navigation.PushAsync(new SettingUserPage(user));
        ListView.SelectedItem = null;
    }
}