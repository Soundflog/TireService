using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Services;

public partial class ServicesPage : ContentPage
{
    public ServicesPage()
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
        var result = await ServiceModel.GetServices();
        ListView.ItemsSource = result;
    }

    // Перейти на страницу добавления услуги
    private async void AddButton_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddServicePage());
    }

    // Перейти на страницу просмотра и изменения услуги
    private async void ListView_OnItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null) return;
        var service = (Service)e.SelectedItem;
        await Navigation.PushAsync(new SettingServicePage(service));
        ListView.SelectedItem = null;
    }
}