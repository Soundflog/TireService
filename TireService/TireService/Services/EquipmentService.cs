using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TireService.Models;

namespace TireService.Services;

public class EquipmentService
{
    private readonly IMongoCollection<Equipment> _equipmentCollection;

    public EquipmentService(
        IOptions<DatabaseSettings> equipmentDbSettings)
    {
        var mongoClient = new MongoClient(
            equipmentDbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            equipmentDbSettings.Value.DatabaseName);

        _equipmentCollection = mongoDatabase.GetCollection<Equipment>(
            equipmentDbSettings.Value.EquipmentCollectionName);
    }

    public async Task<List<Equipment>> GetAllAsync() =>
        await _equipmentCollection.Find(_ => true).ToListAsync();

    // Фильтр по филлиалу
    public async Task<List<Equipment>> GetByBranch(string idBranch)
    {
        return await _equipmentCollection
            .Find(x =>x.Deleted != true && x.BranchId!.Id == idBranch)
            .ToListAsync();
    }
    
    public async Task<Equipment> GetAsync(string id) =>
        await _equipmentCollection
            .Find(x =>x.Deleted != true && x.Id == id)
            .FirstOrDefaultAsync();

    public async Task CreateAsync(Equipment newEquipment) =>
        await _equipmentCollection.InsertOneAsync(newEquipment);

    public async Task UpdateAsync(string id, Equipment updatedEquipment) =>
        await _equipmentCollection.ReplaceOneAsync(x => x.Id == id, updatedEquipment);

    public async Task RemoveAsync(string id) =>
        await _equipmentCollection.DeleteOneAsync(x => x.Id == id);
}