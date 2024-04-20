using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TireService.Models;

namespace TireService.Services;

public class WorkerService 
{
    private readonly IMongoCollection<Worker> _workerCollection;
    
    public WorkerService(
        IOptions<DatabaseSettings> workerDbSettings)
    {
        var mongoClient = new MongoClient(
            workerDbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            workerDbSettings.Value.DatabaseName);

        _workerCollection = mongoDatabase.GetCollection<Worker>(
            workerDbSettings.Value.WorkerCollectionName);
        
    }
    public async Task<List<Worker>> GetAllAsync()=>
        await _workerCollection.Find(_ => true).ToListAsync();

    // Фильтр по филлиалу
    public async Task<List<Worker>?> GetByBranch(string idBranch)
    {
        return await _workerCollection
            .Find(x => x.Deleted != true && x.BranchId!.Id == idBranch)
            .ToListAsync();
    }

    public async Task<Worker> GetAsync(string id) =>
        await _workerCollection.Find(x => x.Deleted != true && x.Id == id).FirstOrDefaultAsync();
    
    public async Task CreateAsync(Worker newWorker) => 
        await _workerCollection.InsertOneAsync(newWorker);

    public async Task UpdateAsync(string id, Worker updatedWorker) =>
        await _workerCollection.ReplaceOneAsync(x => x.Id == id, updatedWorker);

    public async Task RemoveAsync(string id) =>
        await _workerCollection.DeleteOneAsync(x => x.Id == id);
}