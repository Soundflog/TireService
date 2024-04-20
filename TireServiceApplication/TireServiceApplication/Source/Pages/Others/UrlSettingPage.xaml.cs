using TireServiceApplication.Source.Data;

namespace TireServiceApplication.Source.Pages.Others;

public partial class UrlSettingPage : ContentPage
{
    public UrlSettingPage()
    {
        InitializeComponent();
    }

    // Изменить основную ссылку
    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        if (UrlEntry == null || UrlEntry.Text == "")
        {
            await DisplayAlert("Ошибка", "Поле не может быть пустым", "Oк");
            return;
        }
        Storage.SaveUrl(UrlEntry.Text);
        await Navigation.PopAsync();
    }
}