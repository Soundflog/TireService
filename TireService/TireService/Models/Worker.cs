using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TireService.Models;
// Сотрудник
public class Worker
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = null!;
    
    [DefaultValue(false)]
    public bool? Deleted { get; set; } = null!;
    
    [DefaultValue(null)]
    public string? FirstName { get; set; } = null!;
    
    [DefaultValue(null)]
    public string? LastName { get; set; } = null!;
    
    [DefaultValue(null)]
    public string? MidName { get; set; } = null!;
    
    [DefaultValue(null)]
    public DateTime? DateOfBirthday { get; set; } = null!;
    
    [DefaultValue(null)]
    public string? NumberPhone { get; set; } = null!;
    
    [DefaultValue(null)]
    public Branch? BranchId { get; set; } = null!;
    
    // Постоянная оплата за 1 час работы 
    [DefaultValue(null)]
    public double? PayPerHour { get; set; } = null!;
}