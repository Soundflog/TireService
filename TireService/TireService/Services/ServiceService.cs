using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TireService.Models;

namespace TireService.Services;

public class ServiceService
{
    private readonly IMongoCollection<Service> _serviceCollection;

    public ServiceService(
        IOptions<DatabaseSettings> serviceDbSettings)
    {
        var mongoClient = new MongoClient(
            serviceDbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            serviceDbSettings.Value.DatabaseName);

        _serviceCollection = mongoDatabase.GetCollection<Service>(
            serviceDbSettings.Value.ServiceCollectionName);
    }

    public async Task<List<Service>> GetAllAsync() =>
        await _serviceCollection.Find(_ => true).ToListAsync();

    // Фильтр по филлиалу
    public async Task<List<Service>?> GetByBranch(string idBranch)
    {
        return await _serviceCollection
            .Find(x => x.Deleted != true && x.BranchId!.Id == idBranch)
            .ToListAsync();
    }
    
    public async Task<Service?> GetAsync(string id) =>
        await _serviceCollection
            .Find(x => x.Deleted != true &&x.Id == id)
            .FirstOrDefaultAsync();

    public async Task CreateAsync(Service newService) =>
        await _serviceCollection.InsertOneAsync(newService);

    public async Task UpdateAsync(string id, Service updatedService) =>
        await _serviceCollection.ReplaceOneAsync(x => x.Id == id, updatedService);

    public async Task RemoveAsync(string id) =>
        await _serviceCollection.DeleteOneAsync(x => x.Id == id);
}