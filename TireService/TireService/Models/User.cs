using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TireService.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = null!;
    
    [DefaultValue(false)]
    public bool? Deleted { get; set; } = null!;
    
    public string Login { get; set; }
    
    public string Password { get; set; }
    
    [DefaultValue("manager")]
    public string? Role { get; set;} 
    
    [DefaultValue(null)]
    public Branch? BranchId { get; set; } = null!;
    
}