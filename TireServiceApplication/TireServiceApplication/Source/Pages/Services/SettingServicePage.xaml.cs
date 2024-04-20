using TireServiceApplication.Source.Data;
using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Services;

public partial class SettingServicePage : ContentPage
{
    private Service _service;
    private Equipment _equipmentEmpty = new Equipment { Title = "Не выбрано" };
    private List<Equipment> _equipments;
    private List<Picker> _pickerFields = new List<Picker>();

    public SettingServicePage(Service service)
    {
        InitializeComponent();
        _service = service;
        StackLayout.BindingContext = _service;
        _equipments = new List<Equipment> { _equipmentEmpty };
        GetEquipmentData(InitializePicker);
    }

    protected override void OnAppearing()
    {
        DeleteButton.IsVisible = Storage.GetRole() == "admin";
        base.OnAppearing();
    }

    private async void GetEquipmentData(Action initializePicker)
    {
        _equipments.AddRange(await EquipmentModel.GetEquipments() ?? throw new InvalidOperationException());
        initializePicker.Invoke();
    }
    
    // Инициализация списков выбора оборудования
    private void InitializePicker()
    {
        if (_service.EquipmentId == null) return;
        foreach (var equipment in _service.EquipmentId)
        {
            foreach (var equipmentPicker in _equipments)
            {
                if (equipment != null && equipment.Id == equipmentPicker.Id)
                {
                    var picker = AddNewPicker();
                    picker.SelectedItem = equipmentPicker;
                    _pickerFields.Add(picker);
                }
            }
        }

        UpdatePicker();
    }
    
    
    // Формирование выбора оборудования
    private Picker AddNewPicker()
    {
        var picker = new Picker
        {
            ItemsSource = _equipments,
            ItemDisplayBinding = new Binding("Title"),
            Title = "Выберите оборудование",
            FontSize = 18,
            SelectedItem = _equipmentEmpty
        };
        picker.SelectedIndexChanged += (sender, args) => UpdatePicker();
        return picker;
    }
    
    // Вывести на экран список оборудования и дополнительный список выборка оборудования
    private void DisplayPicker()
    {
        StackLayoutPicker.Children.Clear();
        
        var label = new Label()
        {
            Text = "Оборудование:",
            HorizontalOptions = LayoutOptions.Center,
            FontSize = 18
        };
        StackLayoutPicker.Add(label);
        foreach (var picker in _pickerFields)
        {
            StackLayoutPicker.Add(picker);
        }
    }

    // Обновить списки оборудования на странице
    private void UpdatePicker()
    {
        var tempPickerFields = new List<Picker>();
        foreach (var picker in _pickerFields)
        {
            var equipment = (Equipment)picker.SelectedItem;
            if (equipment.Title != _equipmentEmpty.Title)
            {
                tempPickerFields.Add(picker);
            }
        }

        tempPickerFields.Add(AddNewPicker());
        _pickerFields = tempPickerFields;
        DisplayPicker();
    }
    
    private async void SettingButton_OnClicked(object? sender, EventArgs e)
    {
        // Проверка заполнения всех полей
        if (string.IsNullOrEmpty(_service.Title))
        {
            await DisplayAlert("Внимание", "Поле \"Название\" должно быть заполнено!", "Ок");
            return;
        }

        if (_service.Cost == null)
        {
            await DisplayAlert("Внимание", "Поле \"Цена\" должно быть заполнено!", "Ок");
            return;
        }
        if (_service.DurationInMinutes == null)
        {
            await DisplayAlert("Внимание", "Поле \"Длительность\" должно быть заполнено!", "Ок");
            return;
        }
        
        // Считать оборудование из всех списков, которые не нулевые
        var temp = new List<Equipment>();
        foreach (var picker in _pickerFields)
        {
            var equipment = picker.SelectedItem as Equipment;
            if (equipment != null && equipment.Title != _equipmentEmpty.Title) temp.Add(equipment);
        }

        _service.EquipmentId = temp.ToArray();
        
        var result = await ServiceModel.UpdateService(_service);
        if (result) await DisplayAlert("Внимание", "Не удалось изменить услугу.", "Ок");
        await Navigation.PopAsync();
    }
    
    // Удалить услугу
    private async void DeleteButton_OnClicked(object? sender, EventArgs e)
    {
        var alert = await DisplayAlert("Подтверждение", "Удалить услугу?", "Ок", "Отмена");
        if (!alert) return;
        var result = await ServiceModel.DeleteService(_service);
        if (!result) await DisplayAlert("Внимание", "Не удалось удалить услугу.", "Ок");

        await Navigation.PopAsync();
        
    }
}