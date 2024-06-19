using Olx.Models;

namespace Olx.ViewModels;

public class PublicationsViewModel
{
    public IEnumerable<Product> Products { get; set; }
    
    public PublicationState State { get; set; }

    public double Balance { get; set; }
    public int ActiveCount { get; set; }
    public int ArchivedCount { get; set; }
    public int HiddenCount { get; set; }
}