using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Workers;

public partial class AddWorkerPage : ContentPage
{
    private Worker _worker;
    public AddWorkerPage()
    {
        InitializeComponent();
        _worker = new Worker();
        StackLayout.BindingContext = _worker;
    }

    // Добавление сотрудника
    private async void AddNewItemButton_OnClicked(object sender, EventArgs e)
    {
        // Проверка заполнения всех полей
        if (string.IsNullOrEmpty(_worker.FirstName))
        {
            await DisplayAlert("Внимание", "Поле \"Имя\" должно быть заполнено!", "Ок");
            return;
        }
        if (string.IsNullOrEmpty(_worker.LastName))
        {
            await DisplayAlert("Внимание", "Поле \"Фамилия\" должно быть заполнено!", "Ок");
            return;
        }
        if (string.IsNullOrEmpty(_worker.NumberPhone))
        {
            await DisplayAlert("Внимание", "Поле \"Телефон\" должно быть заполнено!", "Ок");
            return;
        }
        
        var result = await WorkerModel.AddWorker(_worker);
        if (result == false) await DisplayAlert("Внимание", "Не удалось добавить работника.", "Ок");
        await Navigation.PopAsync();
    }
}