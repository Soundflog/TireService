using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TireService.Models;
// Услуга
public class Service
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = null!;
    
    [DefaultValue(false)]
    public bool? Deleted { get; set; } = null!;
    
    [DefaultValue(null)]
    public string? Title { get; set; } = null!;
    
    [DefaultValue(null)]
    public double? Cost { get; set; } = null!;
    
    // Продолжительность услуги в минутах
    [DefaultValue(null)]
    public int? DurationInMinutes { get; set; } = null!;
    
    [DefaultValue(null)]
    public Branch? BranchId { get; set; } = null!;
    
    [DefaultValue(null)]
    public Equipment?[]? EquipmentId { get; set; } = null!;
    
}