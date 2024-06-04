using System.Collections;
using Olx.Models;

namespace Olx.ViewModels;

public class HomeViewModel
{
    public int UsersCount { get; set; }
    public int ProductsCount { get; set; }
    public IEnumerable<Category> Categories { get; set; }
    public IEnumerable<Product> VipProducts { get; set; }
}