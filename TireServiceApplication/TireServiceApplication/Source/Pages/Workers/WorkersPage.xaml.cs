using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Workers;

public partial class WorkersPage : ContentPage
{
    public WorkersPage()
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
        var result = await WorkerModel.GetWorkers();
        ListView.ItemsSource = result;
    }

    // Переход на страницу добавления сотрудника
    private async void AddButton_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddWorkerPage());
    }

    // Переход на страницу изменения пользователя
    private async void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null) return;
        var worker = (Worker)e.SelectedItem;
        await Navigation.PushAsync(new SettingWorkerPage(worker));
        ListView.SelectedItem = null;
    }
}
