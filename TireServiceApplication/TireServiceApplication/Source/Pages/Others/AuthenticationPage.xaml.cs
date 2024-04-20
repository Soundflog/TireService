using TireServiceApplication.Source.Data;
using TireServiceApplication.Source.Models;
using TireServiceApplication.Source.Pages.Branches;

namespace TireServiceApplication.Source.Pages.Others;

public partial class AuthenticationPage : ContentPage
{
    public AuthenticationPage()
    {
        InitializeComponent();
    }

    // При активации этой страницы - заблокировать возможность перехода на основную страницу
    protected override async void OnAppearing()
    {
        IsVisible = false;
        var result = await AuthenticationModel.CheckToken();
        if (result) App.SetMainPage();
        base.OnAppearing();
        IsVisible = true;
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    // Авторизация и переход на основную страницу приложению
    private async void AuthenticationButtonOnClicked(object? sender, EventArgs e)
    {
        // Проверка заполнения всех полей
        if (LoginEntry.Text is "" or null)
        {
            await DisplayAlert("Внимание", "Поле \"Логин\" должно быть заполнено!", "Ок");
        }
        else if (PasswordEntry.Text is "" or null)
        {
            await DisplayAlert("Внимание", "Поле \"Пароль\" должно быть заполнено!", "Ок");
        }
        else
        {
            var result = await AuthenticationModel.Authentication(LoginEntry.Text, PasswordEntry.Text);
            if (result == string.Empty)
            {
                App.SetMainPage();
            }
            else
            {
                await DisplayAlert("Внимание", result, "Ок");
            }
        }
    }
}