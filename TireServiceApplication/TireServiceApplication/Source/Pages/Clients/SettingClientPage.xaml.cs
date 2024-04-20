using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Clients;

public partial class SettingClientPage : ContentPage
{
    private Client _client;
    private List<Car> _cars = new List<Car>();
    public SettingClientPage(Client client)
    {
        InitializeComponent();
        _client = client;
        StackLayout.BindingContext = _client;
        if (_client.CarId != null) _cars = _client.CarId.ToList();
        RefreshData();
    }

    // Изменение клиента
    private async void SettingButton_OnClicked(object sender, EventArgs e)
    {
        
        // Проверка заполнения всех полей и выведение сообщения о конкретном поле
        if (string.IsNullOrEmpty(_client.FirstName))
        {
            await DisplayAlert("Внимание", "Поле \"Имя\" должно быть заполнено!", "Ок");
            return;
        }
        if (string.IsNullOrEmpty(_client.LastName))
        {
            await DisplayAlert("Внимание", "Поле \"Фамилия\" должно быть заполнено!", "Ок");
            return;
        }
        if (string.IsNullOrEmpty(_client.NumberPhone))
        {
            await DisplayAlert("Внимание", "Поле \"Номер телефона\" должно быть заполнено!", "Ок");
            return;
        }
        if (_cars.Count > 0)
        {
            foreach (var car in _cars)
            {
                if (string.IsNullOrEmpty(car.CarNumber) && string.IsNullOrEmpty(car.CarBrand) &&
                    string.IsNullOrEmpty(car.CarModel))
                {
                    _cars.Remove(car);
                }
                else if (string.IsNullOrEmpty(car.CarNumber))
                {
                    await DisplayAlert("Внимание", "Поле \"Гос. номер\" должно быть заполнено!", "Ок");
                    return;
                }
                else if (string.IsNullOrEmpty(car.CarBrand))
                {
                    await DisplayAlert("Внимание", "Поле \"Бренд\" должно быть заполнено!", "Ок");
                    return;
                }
                else if (string.IsNullOrEmpty(car.CarModel))
                {
                    await DisplayAlert("Внимание", "Поле \"Модель\" должно быть заполнено!", "Ок");
                    return;
                }
            }
        }

        _client.CarId = _cars.ToArray();

        var result = await ClientModel.UpdateClient(_client);
        if (result) await DisplayAlert("Внимание", "Не удалось изменить клиента.", "Ок");
        
        await Navigation.PopAsync();
    }

    // Удаление клиента
    private async void DeleteButton_OnClicked(object sender, EventArgs e)
    {
        var alert = await DisplayAlert("Подтверждение", "Удалить клиента?", "Ок", "Отмена");
        if (!alert) return;
        var result = await ClientModel.DeleteClient(_client);
        if (!result) await DisplayAlert("Внимание", "Не удалось удалить клиента.", "Ок");
        
        await Navigation.PopAsync();
    }

    private async void AddNewCarButton_OnClicked(object sender, EventArgs e)
    {
        if (_cars.Count > 0)
        {
            foreach (var car in _cars)
            {
                if (string.IsNullOrEmpty(car.CarNumber) && string.IsNullOrEmpty(car.CarBrand) &&
                    string.IsNullOrEmpty(car.CarModel))
                {
                    _cars.Remove(car);
                }
                else if (string.IsNullOrEmpty(car.CarNumber))
                {
                    await DisplayAlert("Внимание", "Поле \"Гос. номер\" должно быть заполнено!", "Ок");
                    return;
                }
                else if (string.IsNullOrEmpty(car.CarBrand))
                {
                    await DisplayAlert("Внимание", "Поле \"Бренд\" должно быть заполнено!", "Ок");
                    return;
                }
                else if (string.IsNullOrEmpty(car.CarModel))
                {
                    await DisplayAlert("Внимание", "Поле \"Модель\" должно быть заполнено!", "Ок");
                    return;
                }
            }
        }

        _cars.Add(new Car());
        RefreshData();
    }
    
    private void RefreshData()
    {
        EntryListView.ItemsSource = null;
        EntryListView.ItemsSource = _cars;
    }
}