using TireService.Models;

namespace TireService.xlsOrders;

public class ReportOrder
{
    public string Address { get; set; }
    public int CountAll { get; set; }
    public int CountSuccesses { get; set; }
    public int CountUnsuccesses { get; set; }
    public double CostUnsuccesses { get; set; }
    public double CostSuccesses { get; set; }
    public double Amortization { get; set; }
    public double Payroll { get; set; }
    public double BonusPayroll { get; set; }
    public double Profit { get; set; }
    public Worker WorkerId { get; set; }
    
}