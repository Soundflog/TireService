using System.ComponentModel;
using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;
using EventArgs = System.EventArgs;

namespace TireServiceApplication.Source.Pages.Clients
{
    public partial class AddClientPage
    {
        private Client _client;
        private List<Car> _cars = new List<Car>();

        public AddClientPage()
        {
            InitializeComponent();
            _client = new Client();
            BindingContext = _client;
        }

        private async void AddNewItemButton_OnClicked(object sender, EventArgs e)
        {
            // Проверка заполнения всех полей и вывод соответствующего сообщения
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

            var result = await ClientModel.AddClient(_client);
            if (result == false) await DisplayAlert("Внимание", "Не удалось добавить клиента.", "Ок");
            await Navigation.PopAsync();
        }

        private async void AddNewCarButton_OnClicked(object sender, EventArgs e)
        {
            var tempCars = new List<Car>();
            if (_cars.Count > 0)
            {
                foreach (var car in _cars)
                {
                    if (string.IsNullOrEmpty(car.CarNumber) && string.IsNullOrEmpty(car.CarBrand) &&
                        string.IsNullOrEmpty(car.CarModel)) continue;

                    if (string.IsNullOrEmpty(car.CarNumber))
                    {
                        await DisplayAlert("Внимание", "Поле \"Гос. номер\" должно быть заполнено!", "Ок");
                        return;
                    }
                    if (string.IsNullOrEmpty(car.CarBrand))
                    {
                        await DisplayAlert("Внимание", "Поле \"Бренд\" должно быть заполнено!", "Ок");
                        return;
                    }
                    if (string.IsNullOrEmpty(car.CarModel))
                    {
                        await DisplayAlert("Внимание", "Поле \"Модель\" должно быть заполнено!", "Ок");
                        return;
                    }

                    tempCars.Add(car);
                }
            }

            tempCars.Add(new Car());
            _cars = tempCars;
            RefreshData();
        }

        private void RefreshData()
        {
            EntryListView.ItemsSource = null;
            EntryListView.ItemsSource = _cars;
        }
    }
}
