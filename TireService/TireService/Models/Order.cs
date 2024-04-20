
using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TireService.Models;
// Заказ
public class Order 
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = null!;
    
    [DefaultValue(false)]
    public bool? Deleted { get; set; } = null!;
    
    [DefaultValue(null)]
    public double? Cost { get; set; } = null!;
    
    [DefaultValue(null)]
    public string? Status { get; set; } = null!;

    [DefaultValue(null)]
    public DateTime? StartDate { get; set; } = null!;
    
    [DefaultValue(null)]
    public DateTime? EndDate { get; set; } = null!;
    
    [DefaultValue(null)]
    public Worker? WorkerId { get; set; } = null!;
    
    [DefaultValue(null)]
    public Service?[]? ServiceId { get; set; } = null!;

}