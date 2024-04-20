using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Users;

public partial class SettingUserPage : ContentPage
{

    private User _user;
    private List<Role> _roles = new List<Role>();
    private List<Branch> _branches = new List<Branch>();

    public SettingUserPage(User user)
    {
        InitializeComponent();
        _user = user;
        StackLayout.BindingContext = _user;
        InitializePicker();
    }

    // Инициализировать поле выбора роли и установка роли выбранного пользователя
    private async void InitializePicker()
    {
        _roles = new List<Role>
        {
            new Role { Name = "Менеджер", Key = "manager" },
            new Role { Name = "Администратор", Key = "admin" }
        };
        PickerRole.ItemsSource = _roles;
        PickerRole.ItemDisplayBinding = new Binding("Name");
        foreach (var role in _roles)
        {
            if (role.Key == _user.Role)
            {
                PickerRole.SelectedItem = role;
            }
        }

        _branches = new List<Branch>();
        _branches.AddRange(await BranchModel.GetBranches() ?? throw new InvalidOperationException());
        PickerBranch.ItemsSource = _branches;
        PickerBranch.ItemDisplayBinding = new Binding("Adress");
        foreach (var branch in _branches)
        {
            if (_user.BranchId != null && branch.Id == _user.BranchId.Id)
            {
                PickerBranch.SelectedItem = branch;
            }
        }
    }

    // Изменение пользователя
    private async void SettingButton_OnClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_user.Login))
        {
            await DisplayAlert("Внимание", "Поле \"Введите логин\" должно быть заполнено!", "Ок");
            return;
        }
        if (string.IsNullOrEmpty(_user.Password))
        {
            await DisplayAlert("Внимание", "Поле \"Введите пароль\" должно быть заполнено!", "Ок");
            return;
        }
        _user.BranchId = (Branch)PickerBranch.SelectedItem;
        _user.Role = ((Role)PickerRole.SelectedItem).Key;
        var result = await UserModel.UpdateUser(_user);
        if (result) await DisplayAlert("Внимание", "Не удалось изменить пользователя.", "Ок");
        await Navigation.PopAsync();
    }

    // Удаление пользователя
    private async void DeleteButton_OnClicked(object? sender, EventArgs e)
    {
        var alert = await DisplayAlert("Подтверждение", "Удалить пользователя?", "Ок", "Отмена");
        if (!alert) return;
        var result = await UserModel.DeleteUser(_user);
        if (!result) await DisplayAlert("Внимание", "Не удалось удалить пользователя.", "Ок");
        await Navigation.PopAsync();
    }
}