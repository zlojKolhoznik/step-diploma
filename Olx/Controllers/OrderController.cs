using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olx.Data;
using Olx.Models;
using Olx.ViewModels;

namespace Olx.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly ShopDbContext _context;

    public OrderController(ShopDbContext context)
    {
        _context = context;
    }

    [Route("/delivery/{id:int:min(1)}")]
    public IActionResult Delivery(int? id)
    {
        var product = _context.Products.Include(product => product.Category)
            .ThenInclude(category => category.Parent).Include(p => p.Owner)
            .Include(p => p.Category).FirstOrDefault(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }
        
        var order = new Order
        {
            ProductId = product.Id,
            Product = product,
            BuyerId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Buyer = _context.Users.Find(User.FindFirstValue(ClaimTypes.NameIdentifier))
        };
        
        List<Category> categoryChain = [];
        var category = product.Category;
        while (category is not null)
        {
            categoryChain.Add(category);
            category = category.Parent;
        }
        
        var viewModel = new OrderViewModel
        {
            Order = order,
            Title = "Доставка",
            Categories = categoryChain
        };
        
        ViewData["OrderViewModel"] = viewModel;
        return View(order);
    }
}