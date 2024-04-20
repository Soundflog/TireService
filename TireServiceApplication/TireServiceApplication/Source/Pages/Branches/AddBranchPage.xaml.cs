using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Branches;

public partial class AddBranchPage : ContentPage
{
    private Branch _branch;
    public AddBranchPage()
    {
        InitializeComponent();
        _branch = new Branch();
        StackLayout.BindingContext = _branch;
    }

    // Добавление нового элемента
    private async void AddNewItemButton_OnClicked(object? sender, EventArgs e)
    {

        if (string.IsNullOrEmpty(_branch.Adress))
        {
            await DisplayAlert("Внимание", "Поле \"Адрес\" должно быть заполнено!", "Ок");
            return;
        }
        
        var result = await BranchModel.AddBranch(_branch);
        if (result == false) await DisplayAlert("Внимание", "Не удалось добавить филиал.", "Ок");
        await Navigation.PopAsync();
    }
}