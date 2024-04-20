using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Users;

public partial class AddUserPage : ContentPage
{
    private User _user;
    private List<Role> _roles = new List<Role>();
    private List<Branch> _branches = new List<Branch>();
    public AddUserPage()
    {
        InitializeComponent();
        _user = new User();
        StackLayout.BindingContext = _user;
        InitializePicker();
    }

    // Инициализировать поле выбора роли
    private async void InitializePicker()
    {
        _roles = new List<Role>
        {
            new Role { Name = "Менеджер", Key = "manager" },
            new Role { Name = "Администратор", Key = "admin" }
        };
        PickerRole.ItemsSource = _roles;
        PickerRole.ItemDisplayBinding = new Binding("Name");
        PickerRole.SelectedItem = _roles.First();

        _branches = new List<Branch>();
        _branches.AddRange(await BranchModel.GetBranches() ?? throw new InvalidOperationException());
        PickerBranch.ItemsSource = _branches;
        PickerBranch.ItemDisplayBinding = new Binding("Adress");
        PickerBranch.SelectedItem = _branches.First();
    }

    // Добавить пользователя
    private async void AddNewItemButton_OnClicked(object? sender, EventArgs e)
    {
        // Проверка заполнения всех полей
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
        var result = await UserModel.AddUser(_user);
        if (result == false) await DisplayAlert("Внимание", "Не удалось добавить пользователя.", "Ок");
        await Navigation.PopAsync();
    }
    

}