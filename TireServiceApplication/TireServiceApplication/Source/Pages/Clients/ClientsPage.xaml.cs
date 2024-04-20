using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Clients;

public partial class ClientsPage : ContentPage
{
    public ClientsPage()
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
        var result = await ClientModel.GetClients();
        ListView.ItemsSource = result;
    }

    // Перейти на страницу добавления клиента
    private async void AddButton_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddClientPage());
    }

    // Перейти на страницу просмотра и изменения клиента
    private async void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null) return;
        var client = (Client)e.SelectedItem;
        await Navigation.PushAsync(new SettingClientPage(client));
        ListView.SelectedItem = null;
    }
}