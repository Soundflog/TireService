using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Equipments;

public partial class AddEquipmentPage : ContentPage
{
    private Equipment _equipment;
    public AddEquipmentPage()
    {
        InitializeComponent();
        _equipment = new Equipment();
        StackLayout.BindingContext = _equipment;
    }

    // Добавление оборудования в БД
    private async void AddNewItemButton_OnClicked(object? sender, EventArgs e)
    {
        // Проверка заполнения всех полей (Название, Стоимость, Покупка, Сборка, Окончание гарантии, Срок службы в годах)
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
        var result = await EquipmentModel.AddEquipment(_equipment);
        if (result == false) await DisplayAlert("Внимание", "Не удалось добавить оборудование.", "Ок");
        await Navigation.PopAsync();
    }
}