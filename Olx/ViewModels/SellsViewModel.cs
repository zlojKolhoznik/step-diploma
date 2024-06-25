using Olx.Models;

namespace Olx.ViewModels;

public class SellsViewModel
{
    public IEnumerable<Product> Products { get; set; }
    
    public PublicationState State { get; set; }

    public double Balance { get; set; }
    public int ActiveCount { get; set; }
    public int ArchivedCount { get; set; }
    public int HiddenCount { get; set; }
    public int OrdersCount { get; set; }

    public int RejectedCount { get; set; }

    public bool Rejected { get; set; }
}