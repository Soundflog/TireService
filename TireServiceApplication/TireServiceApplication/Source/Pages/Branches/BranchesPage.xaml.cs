using TireServiceApplication.Source.Data;
using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Branches;

public partial class BranchesPage : ContentPage
{
    public BranchesPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        RefreshData();
        base.OnAppearing();
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    private async void RefreshData()
    {
        ListView.ItemsSource = null;
        var result = await BranchModel.GetBranches();
        ListView.ItemsSource = result;
    }    

    // Открыть страницу добавления нового элемента
    private async void AddButton_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddBranchPage());
    }

    // Открыть страницу просмотра и изменения филиала, или выбрать филиал как основной у администратора, который нажал на филиал
    // При открытии страницы просмотра филиала - передается филиал, на который нажали
    private async void ListView_OnItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null) return;
        var branch = (Branch)e.SelectedItem;
        var alert = await DisplayAlert("Филиал", branch.Adress, "Выбрать", "Изменить");
        if (alert)
        {
            var user = await UserModel.GetUser(Storage.GetId());
            if (user != null)
            {
                user.BranchId = branch;
                await UserModel.UpdateUser(user);
            }

            await Navigation.PopAsync();
            return;
        }

        await Navigation.PushAsync(new SettingBranchPage(branch));
        ListView.SelectedItem = null;
    }
}