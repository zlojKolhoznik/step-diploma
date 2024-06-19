using System.Diagnostics;
using FuzzySharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olx.Data;
using Olx.Models;
using Olx.Services.Abstract;
using Olx.ViewModels;
using WebApplication1.Models;

namespace Olx.Controllers;

public class HomeController : Controller
{
    private const int PageSize = 6;
    
    private readonly ILogger<HomeController> _logger;
    private readonly ShopDbContext _context;
    private readonly IDeliveryProvider _deliveryProvider;

    public HomeController(ILogger<HomeController> logger, ShopDbContext context, IDeliveryProvider deliveryProvider)
    {
        _logger = logger;
        _context = context;
        _deliveryProvider = deliveryProvider;
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

    public async Task<IActionResult> Search(string? q, string? l, string? minPrice, string? maxPrice, string[]? itemState,
        string? category, int page = 1, string? orderBy = "Relevance")
    {
        var query = _context.Products.Include(p => p.Category)
            .Include(p => p.Filters)
            .AsQueryable();
        City? city = null;
        if (l is not null)
        {
            city = (await _deliveryProvider.GetCitiesAsync(l)).FirstOrDefault();
            if (city is null)
            {
                ViewData["Error"] = "Ми не змогли знайти місто, яке ви ввели :(";
                return RedirectToAction("Index");
            }
        
            query = query.Where(p => p.City == city.Id);
        }
        
        if (minPrice is not null && double.TryParse(minPrice, out var doubleMinPrice))
        {
            query = query.Where(p => p.Price >= doubleMinPrice);
        }
        
        if (maxPrice is not null && double.TryParse(maxPrice, out var doubleMaxPrice))
        {
            query = query.Where(p => p.Price <= doubleMaxPrice);
        }

        if (itemState is not null && itemState.Length == 1 && Enum.TryParse<ItemState>(itemState[0], out var state))
        {
            query = query.Where(p => p.ItemState == state);
        }
        
        if (category is not null)
        {
            query = query.Where(p => p.Category.NormalizedName == category);
        }
        
        var results = query.ToList();
        if (q is not null)
        {
            results = results.Where(p => Fuzz.PartialRatio(p.Name, q) > 70).ToList();
        }
        
        results = Enum.Parse<SortingOrder>(orderBy ?? "Relevance") switch
        {
            SortingOrder.Relevance => q is null ? results : results.OrderByDescending(p => Fuzz.PartialRatio(p.Name, q)).ToList(),
            SortingOrder.NewestFirst => results.OrderByDescending(p => p.CreatedAt).ToList(),
            SortingOrder.CheapestFirst => results.OrderBy(p => p.Price).ToList(),
            SortingOrder.CheapestLast => results.OrderByDescending(p => p.Price).ToList(),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        return View(new SearchViewModel
        {
            Query = q,
            City = city?.Name,
            MinPrice = minPrice ?? "0",
            MaxPrice = maxPrice ?? results.Select(p => p.Price).Max().ToString()!,
            ItemState = itemState,
            Category = category,
            Results = results.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
            Categories = _context.Categories.ToList(),
            Page = page,
            TotalPages = (int)Math.Ceiling(results.Count / (double)PageSize),
            Count = results.Count,
            OrderBy = orderBy
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