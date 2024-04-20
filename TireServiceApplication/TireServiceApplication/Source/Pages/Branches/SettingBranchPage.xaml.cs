using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Branches;

public partial class SettingBranchPage : ContentPage
{
    private Branch _branch;
    public SettingBranchPage(Branch branch)
    {
        InitializeComponent();
        _branch = branch;
        StackLayout.BindingContext = branch;
    }

    // Изменение филиала
    private async void SettingButton_OnClicked(object? sender, EventArgs e)
    {
        // Проверка заполнения всех полей
        if (string.IsNullOrEmpty(_branch.Adress))
        {
            await DisplayAlert("Внимание", "Поле \"Адресс\" должно быть заполнено!", "Ок");
            return;
        }
        var result = await BranchModel.UpdateBranch(_branch);
        if (result) await DisplayAlert("Внимание", "Не удалось изменить филиал.", "Ок");
        
        await Navigation.PopAsync();
    }
    
    // Удаление филиала
    private async void DeleteButton_OnClicked(object? sender, EventArgs e)
    {
        var alert = await DisplayAlert("Подтверждение", "Удалить филиал?", "Ок", "Отмена");
        if (!alert) return;
        var result = await BranchModel.DeleteBranch(_branch);
        if (!result) await DisplayAlert("Внимание", "Не удалось удалить филиал.", "Ок");
        
        await Navigation.PopAsync();
    }
}