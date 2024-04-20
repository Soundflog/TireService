using TireServiceApplication.Source.Data;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Models;

public class ServiceModel
{
    private static List<Service> _services = new List<Service>();
    
    // Метод для получения всех услуг
    public static async Task<List<Service>?> GetServices()
    {
        var services = await ServiceData.GetServices();
        if (services != null)
        {
            _services = services;
            TextForListView(services);
        }

        return _services;
    }
    
    // Метод для добавления услуги
    public static async Task<bool> AddService(Service service)
    {
        var newService = await ServiceData.AddService(service);
        if (newService != null) _services.Add(newService);
        return newService != null;
    }
    
    // Метод для изменения услуги
    public static async Task<bool> UpdateService(Service service)
    {
        var newService = await ServiceData.UpdateService(service);
        if (newService != null)
        {
            var index = _services.FindIndex(x => x.Id == service.Id);
            _services[index] = newService;
        }
        
        return newService != null;
    }
    
    // Метод для удаления услуги
    public static async Task<bool> DeleteService(Service service)
    {
        if (service.Id == null) return false;
        var result = await ServiceData.DeleteService(service.Id);
        if (result)
        {
            var index = _services.FindIndex(x => x.Id == service.Id);
            _services.RemoveAt(index);
        }
        
        return result;
    }
    
    // Метод для составления текста для отображения в ListView
    public static void TextForListView(List<Service> services)
    {
        foreach (var service in services)
        {
            if (service.Cost == null) service.CostView = "Стоимость: 0 руб.";
            else service.CostView = $"Стоимость: {Math.Round((double)service.Cost, 2)} руб.";
            service.DurationInMinutesView = $"Длительность: {service.DurationInMinutes} мин.";
        }
    }
}