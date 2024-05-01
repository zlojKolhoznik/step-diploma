using Microsoft.AspNetCore.Mvc;
using Olx.Models;

namespace Olx.ViewComponents;

public class ProductCard : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(Product product)
    {
        return View(product);
    }
}