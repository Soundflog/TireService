using TireServiceApplication.Source.Entities;
using TireServiceApplication.Source.Models;
using TireServiceApplication.Source.Pages.Clients;

namespace TireServiceApplication.Source.Pages.Orders;

public partial class AddOrderPage : ContentPage
{
    private Order _order;
    private List<Service> _services = new List<Service>();
    private List<Worker> _workers = new List<Worker>();
    private List<Client> _clients = new List<Client>();
    private Service _serviceEmpty = new Service(){Title = "Не выбрано"};
    private Worker _workerEmpty = new Worker(){TitleView = "Не выбрано"};
    private Client _clientEmpty = new Client(){TitleView = "Не выбрано"};
    private List<Picker> _pickerServiceFields = new List<Picker>();
    private Picker _pickerWorkerField = new Picker();
    private Picker _pickerClientField = new Picker();

    public AddOrderPage()
    {
        InitializeComponent();
        _order = new Order();
        StackLayout.BindingContext = _order;
        InitializePage();
    }

    // Инициализация выпадающих списков
    private async void InitializePage()
    {
        await GetWorkerDataPicker();
        await GetServiceDataPicker();
        await GetClientDataPicker();
    }

    // Получить услуги
    private async Task GetServiceDataPicker()
    {
        _services = new List<Service> {_serviceEmpty};
        _services.AddRange(await ServiceModel.GetServices() ?? throw new InvalidOperationException());
        _pickerServiceFields.Add(AddNewServicePicker());

        DisplayServicePicker();
    }
    
    // Получение сотрудников
    private async Task GetWorkerDataPicker()
    {
        _workers = new List<Worker> { _workerEmpty };
        _workers = await WorkerModel.GetWorkers() ?? throw new InvalidOperationException();
        _pickerWorkerField = AddNewWorkerPicker();
        
        DisplayWorkerPicker();
    }
    
    // Получение клиентов
    private async Task GetClientDataPicker()
    {
        _clients = new List<Client> { _clientEmpty };
        _clients = await ClientModel.GetClients() ?? throw new InvalidOperationException();
        _pickerClientField = AddNewClientPicker();
        
        DisplayClientPicker();
    }
    

    // Добавление новой услуги
    private Picker AddNewServicePicker()
    {
        var picker = new Picker
        {
            ItemsSource = _services,
            ItemDisplayBinding = new Binding("Title"),
            Title = "Выберите услугу",
            FontSize = 18,
            SelectedItem = _serviceEmpty
        };
        picker.SelectedIndexChanged += (sender, args) => UpdateServicePicker();
        return picker;
    }

    // Добавление нового сотрудника
    private Picker AddNewWorkerPicker()
    {
        var picker = new Picker
        {
            ItemsSource = _workers,
            ItemDisplayBinding = new Binding("TitleView"),
            Title = "Выберите специалиста",
            FontSize = 18,
            SelectedItem = _workerEmpty
        };
        return picker;
    }
    
    // Добавление нового клиента
    private Picker AddNewClientPicker()
    {
        var picker = new Picker
        {
            ItemsSource = _clients,
            ItemDisplayBinding = new Binding("TitleView"),
            Title = "Выберите клиента",
            FontSize = 18,
            SelectedItem = _clientEmpty
        };
        return picker;
    }
    
    // Проверка услуг
    // Должно быть не больше 5 услуг всего
    // Если услуг меньше 5 - добавить поле для выбора новой услуги
    // При проверке - посчитать цену выбранных услуг
    private void UpdateServicePicker()
    {
        StackLayoutWorkerPicker.Children.Clear();
        var tempPicker = new List<Picker>();
        var tempCost = 0.0;
        foreach (var picker in _pickerServiceFields)
        {
            var service = (Service) picker.SelectedItem;
            if (service.Title != _serviceEmpty.Title)
            {
                tempPicker.Add(picker);
                if (service.Cost == null) continue;
                tempCost += (double)service.Cost;
            }
        }
        // Округлить до 2 знаков после запятой
        CostLabel.Text = $"Стоимость: {Math.Round(tempCost, 2)} руб";
        if (tempPicker.Count < 5) tempPicker.Add(AddNewServicePicker());
        _pickerServiceFields = tempPicker;
        DisplayServicePicker();
    }
    
    // Вывести на экран выбор услуг
    private void DisplayServicePicker()
    {
        StackLayoutServicePicker.Children.Clear();
        StackLayoutWorkerPicker.Children.Clear();
        StackLayoutClientPicker.Children.Clear();
        
        var label = new Label()
        {
            Text = "Услуги: ",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontSize = 18
        };
        StackLayoutServicePicker.Add(label);
        foreach (var picker in _pickerServiceFields)
        {
            StackLayoutServicePicker.Add(picker);
        }
        
        DisplayWorkerPicker();
        DisplayClientPicker();
    }

    // Вывести на экран выбор сотрудников
    private void DisplayWorkerPicker()
    {
        StackLayoutWorkerPicker.Children.Clear();
        
        var label = new Label()
        {
            Text = "Специалист: ",
            VerticalOptions = LayoutOptions.Center,
            FontSize = 18
        };
        StackLayoutWorkerPicker.Add(label);
        StackLayoutWorkerPicker.Add(_pickerWorkerField);
    }
    
    // Вывести на экран выбор клиента
    private void DisplayClientPicker()
    {
        StackLayoutClientPicker.Children.Clear();
        
        var label = new Label()
        {
            Text = "Клиент: ",
            VerticalOptions = LayoutOptions.Center,
            FontSize = 18
        };
        StackLayoutClientPicker.Add(label);
        StackLayoutClientPicker.Add(_pickerClientField);
    }
    
    // Добавление нового заказа
    private async void AddNewItemButton_OnClicked(object? sender, EventArgs e)
    {
        // Проверка заполнения всех полей
        var worker = (Worker) _pickerWorkerField.SelectedItem;
        var client = (Client) _pickerClientField.SelectedItem;
        if (worker.TitleView == _workerEmpty.TitleView)
        {
            await DisplayAlert("Внимание", "Поле \"Гос. номер\" должно быть заполнено!", "Ок");
            return;
        }
        if (_pickerServiceFields.Count < 2)
        {
            await DisplayAlert("Внимание", "Услуги не выбраны!", "Ок");
            return;
        }
        var services = new List<Service>();
         foreach (var picker in _pickerServiceFields)
        {
            var service = (Service) picker.SelectedItem;
            if (service.Title != _serviceEmpty.Title)
            {
                services.Add(service);
            }
        }

        if (client.TitleView == _clientEmpty.TitleView)
        {
            await DisplayAlert("Внимание", "Клиент не выбран!", "Ок");
            _order.ClientId = null;
        }
        else
        {
            _order.ClientId = client;
        }
        _order.WorkerId = worker;
        _order.ServiceId = services.ToArray();
        _order.StartDate = DatePicker.Date + TimePicker.Time;
        var result = await OrderModel.AddOrder(_order);
        if (result == false) await DisplayAlert("Внимание", "Не удалось добавить заказ.", "Ок");
        else await Navigation.PopAsync();
    }

    private async void AddNewClientButton_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddClientPage());
        await GetClientDataPicker();
    }
}