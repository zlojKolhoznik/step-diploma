using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olx.Data;
using Olx.Models;
using Olx.ViewModels;

namespace Olx.Controllers;

[Authorize]
public class PublicationsController : Controller
{
    private readonly ShopDbContext _context;

    public PublicationsController(ShopDbContext context)
    {
        _context = context;
    }

    [Route("/publications/{state=active}")]
    public IActionResult Sells(string state, string? orderBy = "Relevance", bool showOrders = false)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (user is null)
        {
            return Forbid();
        }
        
        if (showOrders)
        {
            var orders = _context.Orders.Include(o => o.Product)
                .Where(o => o.Product.OwnerId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .OrderByDescending(o => o.CreatedAt);
            return View(new SellsViewModel
            {
                Products = orders.Select(o => o.Product).ToList(),
                State = PublicationState.Active,
                Balance = _context.Users.FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier))!.Balance,
                OrdersCount = orders.Count(),
                ActiveCount = _context.Products.Count(p => p.PublicationState == PublicationState.Active && p.OwnerId == user.Id),
                ArchivedCount = _context.Products.Count(p => p.PublicationState == PublicationState.Archived && p.OwnerId == user.Id),
                HiddenCount = _context.Products.Count(p => p.PublicationState == PublicationState.Hidden && p.OwnerId == user.Id)
            });
        }
        
        if (!Enum.TryParse<PublicationState>(state, true, out var publicationState))
        {
            return NotFound();
        }

        var results = _context.Products.Where(p => p.PublicationState == publicationState)
            .Where(p => p.OwnerId == User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (orderBy is not null && Enum.TryParse<SortingOrder>(orderBy, true, out var sortingOrder))
        {
            results = sortingOrder switch
            {
                SortingOrder.Relevance => results,
                SortingOrder.CheapestFirst => results.OrderBy(p => p.Price),
                SortingOrder.CheapestLast => results.OrderByDescending(p => p.Price),
                SortingOrder.NewestFirst => results.OrderByDescending(p => p.CreatedAt),
                _ => results
            };
        }
        
        return View(new SellsViewModel
        {
            Products = results.ToList(),
            State = publicationState,
            Balance = user!.Balance,
            ActiveCount = _context.Products.Count(p => p.PublicationState == PublicationState.Active && p.OwnerId == user.Id),
            ArchivedCount = _context.Products.Count(p => p.PublicationState == PublicationState.Archived && p.OwnerId == user.Id),
            HiddenCount = _context.Products.Count(p => p.PublicationState == PublicationState.Hidden && p.OwnerId == user.Id),
            OrdersCount = _context.Orders.Count(o => o.Product.OwnerId == user.Id),
        });
    }
    
    [Route("/purchases")]
    public IActionResult Purchases(string? state = "Pending")
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
        var purchases = _context.Orders.Include(o => o.Product)
            .Where(o => o.BuyerId == user.Id);
        var orderState = OrderState.Pending;
        if (state is not null && Enum.TryParse(state, true, out orderState))
        {
            purchases = purchases.Where(o => o.State == orderState);
        }
        
        purchases = purchases.OrderByDescending(o => o.CreatedAt);

        return View(new PurchasesViewModel
        {
            Products = purchases.Select(o => o.Product).ToList(),
            State = orderState,
            Balance = user!.Balance,
            PendingCount = _context.Orders.Count(o => o.BuyerId == user.Id && o.State == OrderState.Pending),
            CompletedCount = _context.Orders.Count(o => o.BuyerId == user.Id && o.State == OrderState.Completed),
            CanceledCount = _context.Orders.Count(o => o.BuyerId == user.Id && o.State == OrderState.Canceled),
            InDeliveryCount = _context.Orders.Count(o => o.BuyerId == user.Id && o.State == OrderState.InDelivery),
            InWarehouseCount = _context.Orders.Count(o => o.BuyerId == user.Id && o.State == OrderState.InWarehouse),
            ProcessingCount = _context.Orders.Count(o => o.BuyerId == user.Id && o.State == OrderState.Processing)
        });

    }
    
    
}