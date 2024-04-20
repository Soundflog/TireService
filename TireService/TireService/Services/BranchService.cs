using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TireService.Models;

namespace TireService.Services;

public class BranchService
{
    private readonly IMongoCollection<Branch> _branchCollection;

    public BranchService(
        IOptions<DatabaseSettings> branchDbSettings)
    {
        var mongoClient = new MongoClient(
            branchDbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            branchDbSettings.Value.DatabaseName);

        _branchCollection = mongoDatabase.GetCollection<Branch>(
            branchDbSettings.Value.BranchCollectionName);
    }

    public async Task<List<Branch>> GetAllAsync() =>
        await _branchCollection.Find(_ => true).ToListAsync();
    
    public async Task<Branch?> GetAsync(string id) =>
        await _branchCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Branch newBranch) =>
        await _branchCollection.InsertOneAsync(newBranch);

    public async Task UpdateAsync(string id, Branch updatedBranch) =>
        await _branchCollection.ReplaceOneAsync(x => x.Id == id, updatedBranch);

    public async Task RemoveAsync(string id) =>
        await _branchCollection.DeleteOneAsync(x => x.Id == id);
}