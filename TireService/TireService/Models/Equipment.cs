using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TireService.Models;

// Оборудование
public class Equipment
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
    
    [DefaultValue(null)]
    public DateTime? PurchaseDate { get; set; } = null!;
    
    [DefaultValue(null)]
    public DateTime? BuildDate { get; set; } = null!;
    
    [DefaultValue(null)]
    public DateTime? GuaranteeDate { get; set; } = null!;
    
    [DefaultValue(null)]
    public int? UsefulLifeDate { get; set; } = null!;
    
    [DefaultValue(null)]
    public Branch? BranchId { get; set; } = null!;
    
    // Оценочная стоимость и амортизация по месяцам (массив)
    // [месяц, оценочная стоимость, амортизация в руб]
    [DefaultValue(null)]
    public List<Estimated>? EstimatedValue { get; set; } = null!;
    
    
}