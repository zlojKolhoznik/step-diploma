using Olx.Models;

namespace Olx.ViewModels;

public class OrderViewModel
{
    public Order Order { get; set; }

    public IEnumerable<Category> Categories { get; set; }

    public string Title { get; set; }
}