using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Equipments;

public partial class SettingEquipmentPage : ContentPage
{
    private Equipment _equipment;
    public SettingEquipmentPage(Equipment equipment)
    {
        InitializeComponent();
        _equipment = equipment;
        StackLayout.BindingContext = equipment;
    }

    // Изменение оборудования
    private async void SettingButton_OnClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_equipment.Title))
        {
            await DisplayAlert("Внимание", "Поле \"Название\" должно быть заполнено!", "Ок");
            return;
        }
        if (_equipment.Cost == null)
        {
            await DisplayAlert("Внимание", "Поле \"Стоимость\" должно быть заполнено!", "Ок");
            return;
        }
        if (_equipment.BuildDate == null)
        {
            await DisplayAlert("Внимание", "Поле \"Сборка\" должно быть заполнено!", "Ок");
            return;
        }
        if (_equipment.GuaranteeDate == null)
        {
            await DisplayAlert("Внимание", "Поле \"Окончание гарантии\" должно быть заполнено!", "Ок");
            return;
        }
        if (_equipment.PurchaseDate == null)
        {
            await DisplayAlert("Внимание", "Поле \"Покупка\" должно быть заполнено!", "Ок");
            return;
        }
        if (_equipment.UsefulLifeDate == null)
        {
            await DisplayAlert("Внимание", "Поле \"Срок службы в годах\" должно быть заполнено!", "Ок");
            return;
        }
        
        var result = await EquipmentModel.UpdateEquipment(_equipment);
        if (result) await DisplayAlert("Внимание", "Не удалось изменить оборудование.", "Ок");
        
        await Navigation.PopAsync(); 
    }
    
    // Удаления оборудования
    private async void DeleteButton_OnClicked(object? sender, EventArgs e)
    {
        var alert = await DisplayAlert("Подтверждение", "Удалить оборудование?", "Ок", "Отмена");
        if (!alert) return;
        var result = await EquipmentModel.DeleteEquipment(_equipment);
        if (!result) await DisplayAlert("Внимание", "Не удалось удалить оборудование.", "Ок");
        
        await Navigation.PopAsync();
    }
}