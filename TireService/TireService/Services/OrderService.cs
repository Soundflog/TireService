using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TireService.Models;

namespace TireService.Services;

public class OrderService
{
    private readonly IMongoCollection<Order> _orderCollection;

    public OrderService(
        IOptions<DatabaseSettings> orderDbSettings)
    {
        var mongoClient = new MongoClient(
            orderDbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            orderDbSettings.Value.DatabaseName);

        _orderCollection = mongoDatabase.GetCollection<Order>(
            orderDbSettings.Value.OrderCollectionName);
    }

    public async Task<List<Order>> GetAllAsync() =>
        await _orderCollection.Find(_ => true).ToListAsync();

    // Фильтр по филлиалу
    public async Task<List<Order>> GetByBranch(string idBranch)
    {
        return await _orderCollection
            .Find(x =>x.Deleted != true && 
                      x.WorkerId!.BranchId!.Id == idBranch)
            .ToListAsync();
    }
    // Фильтр текущих заказов по филиалу  
    public async Task<List<Order>> GetByBranchInProcessOrPlan(string idBranch)
    {
        return await _orderCollection
            .Find(x => x.Deleted != true &&
                x.WorkerId!.BranchId!.Id == idBranch 
                && 
                (x.Status == "Plan" || x.Status == "InProcess"))
            .ToListAsync();
    }
    
    // Фильтр прошлых заказов по филиалу  
    public async Task<List<Order>> GetPastByBranch(string idBranch)
    {
        return await _orderCollection
            .Find(x => x.Deleted != true &&
                x.WorkerId!.BranchId!.Id == idBranch
                && 
                (x.Status == "Success" || x.Status == "Unsuccess"))
            .ToListAsync();
    }
    
    // Фильтр по заданному статусу
    public async Task<List<Order>> GetByBranchStatus(string idBranch, string status)
    {
        return await _orderCollection
            .Find(x => x.Deleted != true &&
                       x.WorkerId!.BranchId!.Id == idBranch
                       && 
                       x.Status == status )
            .ToListAsync();
    }
    // Фильтр для отчета по оказанию услуг по id филиала и датам
    public async Task<List<Order>> GetAllByDateBranch(string idBranch, DateTime startDate, DateTime endDate)
    {
        return await _orderCollection
            .Find(x => x.Deleted != true && x.WorkerId!.BranchId!.Id == idBranch 
               && x.EndDate >= startDate && x.EndDate <= endDate)
            .ToListAsync();
    }
    
    public async Task<List<Order>> GetAllByDate(DateTime startDate, DateTime endDate)
    {
        return await _orderCollection
            .Find(x => x.Deleted != true 
                       && x.EndDate >= startDate && x.EndDate <= endDate)
            .ToListAsync();
    }
    
    public async Task<Order?> GetAsync(string id) =>
        await _orderCollection.Find(x =>x.Deleted != true && x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Order newOrder) =>
        await _orderCollection.InsertOneAsync(newOrder);
    
    public async Task UpdateAsync(string id, Order updatedOrder) =>
        await _orderCollection.ReplaceOneAsync(x => x.Id == id, updatedOrder);

    public async Task RemoveAsync(string id) =>
        await _orderCollection.DeleteOneAsync(x => x.Id == id);
}