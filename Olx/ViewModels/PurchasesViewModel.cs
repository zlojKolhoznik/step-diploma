using Olx.Models;

namespace Olx.ViewModels;

public class PurchasesViewModel
{
    public List<Product> Products { get; set; }
    
    public OrderState State { get; set; }
    
    public double Balance { get; set; }
    
    public int PendingCount { get; set; }
    
    public int CompletedCount { get; set; }
    
    public int CanceledCount { get; set; }
    
    public int InDeliveryCount { get; set; }
    
    public int InWarehouseCount { get; set; }
    
    public int ProcessingCount { get; set; }
}