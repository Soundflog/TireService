using TireServiceApplication.Source.Models;
using TireServiceApplication.Source.Pages.Clients;
using TireServiceApplication.Source.Pages.Equipments;
using TireServiceApplication.Source.Pages.Orders;
using TireServiceApplication.Source.Pages.Services;
using TireServiceApplication.Source.Pages.Workers;

namespace TireServiceApplication.Source.Pages.Others;

public partial class MainPage : Shell
{
    private Tab _workersTab;
    private Tab _ordersTab;
    private Tab _servicesTab;
    private Tab _settingsTab;
    private Tab _clientsTab;
    

    public MainPage()
    {
        InitializeComponent();
        InitializeTabBar();
    }

    // Страницы, которые находятся в Таб баре
    private void InitializeTabBar()
    {
        _servicesTab = AddTab(new ServicesPage(), "Услуги");
        _workersTab = AddTab(new WorkersPage(), "Сотрудники");
        _ordersTab = AddTab(new OrdersPage(), "Заказы");
        _clientsTab = AddTab(new ClientsPage(), "Клиенты");
        _settingsTab = AddTab(new SettingsPage(), "Ещё");
        TabBar.Items.Add(_servicesTab);
        TabBar.Items.Add(_workersTab);
        TabBar.Items.Add(_ordersTab);
        TabBar.Items.Add(_clientsTab);
        TabBar.Items.Add(_settingsTab);
        TabBar.CurrentItem = _ordersTab;
    }

    // Формирование новой страницы Tab
    private Tab AddTab(ContentPage contentPage, string title)
    {
        var shellContent = new ShellContent
        {
            ContentTemplate = new DataTemplate(() => contentPage)
        };
        var tab = new Tab { Title = title };
        tab.Items.Add(shellContent);
        return tab;
    }

    protected override void OnAppearing()
    {
        CheckAuthentication();
        base.OnAppearing();
    }

    // Проверка на авторизацию пользователя, при открытии основной страницы, если токен не подходит - открыть страницу авторизации
    private async void CheckAuthentication()
    {
        var result = await AuthenticationModel.CheckToken();
        if (result) return;
        App.SetAuthenticationPage();
    }
}