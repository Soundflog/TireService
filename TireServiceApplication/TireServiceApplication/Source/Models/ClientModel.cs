using TireServiceApplication.Source.Data;
using TireServiceApplication.Source.Entities;

namespace TireServiceApplication.Source.Models;

public class ClientModel
{
    private static List<Client> _clients = new List<Client>();
    
    // Метод для получения всех клиентов
    public static async Task<List<Client>?> GetClients()
    {
        var clients = await ClientData.GetClients();
        if (clients != null)
        {
            _clients = clients;
            TextForListView(_clients);
        }
        return _clients;
    }
    
    // Метод для добавления клиента
    public static async Task<bool> AddClient(Client client)
    {
        var newClient = await ClientData.AddClient(client);
        if (newClient != null) _clients.Add(newClient);
        return newClient != null;
    }
    
    // Метод для изменения клиента
    public static async Task<bool> UpdateClient(Client client)
    {
        var newClient = await ClientData.UpdateClient(client);
        if (newClient != null)
        {
            var index = _clients.FindIndex(x => x.Id == client.Id);
            _clients[index] = newClient;
        }

        return newClient != null;
    }
    
    // Метод для удаления клиента
    public static async Task<bool> DeleteClient(Client client)
    {
        if (client.Id == null) return false;
        var result = await ClientData.DeleteClient(client.Id);
        if (result)
        {
            var index = _clients.FindIndex(x => x.Id == client.Id);
            _clients.RemoveAt(index);
        }

        return result;
    }
    
    // Метод для составления текста для отображения в ListView
    public static void TextForListView(List<Client> clients)
    {
        foreach (var client in clients)
        {
            client.TitleView = $"{client.LastName} {client.FirstName}";
            client.TotalSpentView = $"Потрачено: {Math.Round(client.TotalSpent ?? 0, 2)} руб.";
        }
    }
}