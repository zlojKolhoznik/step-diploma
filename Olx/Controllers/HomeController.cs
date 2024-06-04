using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Olx.Data;
using Olx.ViewModels;
using WebApplication1.Models;

namespace Olx.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ShopDbContext _context;

    public HomeController(ILogger<HomeController> logger, ShopDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View(new HomeViewModel
        {
            UsersCount = _context.Users.Count(),
            ProductsCount = _context.Products.Count(),
            Categories = _context.Categories.ToList(),
            VipProducts = _context.Products.OrderBy(p => Guid.NewGuid()).Take(8).ToList() // Random 8 products. In future, we will change this to VIP products. In real world app, it is better to choose products on user preferences.
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}