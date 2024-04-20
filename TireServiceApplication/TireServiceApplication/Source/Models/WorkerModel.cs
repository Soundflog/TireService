using TireServiceApplication.Source.Data;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Models;

public class WorkerModel
{
    private static List<Worker> _workers = new List<Worker>();

    // Метод для получения всех работников
    public static async Task<List<Worker>?> GetWorkers()
    {
        var workers = await WorkerData.GetWorkers();
        if (workers != null)
        {
            _workers = workers;
            TextForListView(_workers);
        }

        return _workers;
    }

    // Метод для добавления работника
    public static async Task<bool> AddWorker(Worker worker)
    {
        var newWorker = await WorkerData.AddWorker(worker);
        if (newWorker != null) _workers.Add(newWorker);
        return newWorker != null;
    }

    // Метод для изменения работника
    public static async Task<bool> UpdateWorker(Worker worker)
    {
        var newWorker = await WorkerData.UpdateWorker(worker);
        if (newWorker != null)
        {
            var index = _workers.FindIndex(x => x.Id == worker.Id);
            _workers[index] = newWorker;
        }

        return newWorker != null;
    }

    // Метод для удаления работника
    public static async Task<bool> DeleteWorker(Worker worker)
    {
        if (worker.Id == null) return false;
        var result = await WorkerData.DeleteWorker(worker.Id);
        if (result)
        {
            var index = _workers.FindIndex(x => x.Id == worker.Id);
            _workers.RemoveAt(index);
        }

        return result;
    }
    
    // Метод для составления текста для отображения в ListView
    public static void TextForListView(List<Worker> workers)
    {
        foreach (var worker in workers)
        {
            worker.TitleView = $"{worker.LastName} {worker.FirstName} {worker.MidName}";
            worker.NumberPhoneView = $"Телефон: {worker.NumberPhone}";
            worker.PayPerHourView = $"Оплата за час: {Math.Round(worker.PayPerHour ?? 0, 2)} руб/час";
        }
    }
}