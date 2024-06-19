using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public IActionResult Index(string state, string? orderBy = "Relevance")
    {
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

        var user = _context.Users.FirstOrDefault(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        return View(new PublicationsViewModel
        {
            Products = results.ToList(),
            State = publicationState,
            Balance = user!.Balance,
            ActiveCount = _context.Products.Count(p => p.PublicationState == PublicationState.Active && p.OwnerId == user.Id),
            ArchivedCount = _context.Products.Count(p => p.PublicationState == PublicationState.Archived && p.OwnerId == user.Id),
            HiddenCount = _context.Products.Count(p => p.PublicationState == PublicationState.Hidden && p.OwnerId == user.Id)
        });
    }
    
    
}