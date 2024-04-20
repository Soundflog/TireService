using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TireService.Models;

// Филлиал 
public class Branch
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = null!;
    
    [DefaultValue(null)]
    public string? Adress { get; set; } = null!;
    
    [DefaultValue(false)]
    public bool? Deleted { get; set; } = null!;
}