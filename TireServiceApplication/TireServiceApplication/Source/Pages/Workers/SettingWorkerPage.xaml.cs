using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Workers;

public partial class SettingWorkerPage : ContentPage
{
    private Worker _worker;
    public SettingWorkerPage(Worker worker)
    {
        InitializeComponent();
        _worker = worker;
        StackLayout.BindingContext = worker;
    }
    
    // Изменение сотрудника
    private async void SettingButton_OnClicked(object sender, EventArgs e)
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
        var result = await WorkerModel.UpdateWorker(_worker);
        if (result) await DisplayAlert("Внимание", "Не удалось изменить работника.", "Ок");
        
        await Navigation.PopAsync();
    }
    
    // Удаление сотрудника
    private async void DeleteButton_OnClicked(object sender, EventArgs e)
    {
        var alert = await DisplayAlert("Подтверждение", "Удалить работника?", "Ок", "Отмена");
        if (!alert) return;
        var result = await WorkerModel.DeleteWorker(_worker);
        if (!result) await DisplayAlert("Внимание", "Не удалось удалить работника.", "Ок");
        
        await Navigation.PopAsync();
    }

}