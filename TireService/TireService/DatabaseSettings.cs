using MongoDB.Driver;
using TireService.Models;

namespace TireService;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string OrderCollectionName { get; set; } = null!;
    public string ServiceCollectionName { get; set; } = null!;
    public string BranchCollectionName { get; set; } = null!;
    public string WorkerCollectionName { get; set; } = null!;
    public string EquipmentCollectionName { get; set; } = null!;
    public string AuthenticationCollectionName { get; set; } = null!;
    public string ManagerCollectionName { get; set; } = null!;
    public string UserCollectionName { get; set; } = null!;
}