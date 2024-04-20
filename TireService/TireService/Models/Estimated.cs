using System.ComponentModel;

namespace TireService.Models;
[Serializable]
public class Estimated
{
    [DefaultValue(null)]
    public DateTime? Month { get; set; } = null!;
    [DefaultValue(null)]
    public double? EstimatedCost { get; set; } = null!;
    [DefaultValue(null)]
    public double? Amortization { get; set; } = null!;
    
}