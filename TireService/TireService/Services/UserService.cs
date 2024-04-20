using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TireService.Models;

namespace TireService.Services;

public class UserService
{
    private readonly IMongoCollection<User> _usersCollection;

    public UserService(
        IOptions<DatabaseSettings> usersDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            usersDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            usersDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<User>(
            usersDatabaseSettings.Value.UserCollectionName);
    }
    
    public async Task<List<User>> GetAllAsync() =>
        await _usersCollection.Find(_ => true).ToListAsync();

    public async Task<List<User>> GetAllAdmins() =>
        await _usersCollection.Find(x => x.Role == "admin").ToListAsync();

    public async Task<List<User>> GetAllBring() =>
        await _usersCollection.Find(x => x.Deleted != true).ToListAsync();
    
    public async Task<User> GetByLoginPassword(string login, string password) =>
        await _usersCollection
            .Find(u => u.Deleted != true && u.Login == login && u.Password == password)
            .FirstOrDefaultAsync();
    
    public async Task<User> GetByLogin(string login) =>
        await _usersCollection.Find(u => u.Deleted != true && u.Login == login).FirstOrDefaultAsync();
    
    public async Task<User?> GetOneByBranch(string id, string idBranch) =>
        await _usersCollection
            .Find(x => x.Deleted != true && x.Id == id && x.BranchId!.Id == idBranch)
            .FirstOrDefaultAsync();
    public async Task<User?> GetAsync(string id) =>
        await _usersCollection.Find(x => x.Deleted != true && x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(User newUser) =>
        await _usersCollection.InsertOneAsync(newUser);

    public async Task UpdateAsync(string id, User updatedUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

}