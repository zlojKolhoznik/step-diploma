using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olx.Data;
using Olx.Models;
using Olx.Services.Abstract;

namespace Olx.Controllers;

[Authorize]
public class ChatsController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ShopDbContext _dbContext;
    private readonly IPhotoManager _photoManager;

    public ChatsController(UserManager<User> userManager, ShopDbContext dbContext, IPhotoManager photoManager)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _photoManager = photoManager;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var user = (await _userManager.GetUserAsync(User))!;
        var messages = _dbContext.Messages.Include(m => m.Product)
            .ThenInclude(p => p.Owner)
            .Include(m => m.Buyer)
            .Include(m => m.Sender)
            .Where(m => m.Product.OwnerId == user.Id || m.BuyerId == user.Id)
            .GroupBy(m => m.ProductId);
        var productIds = messages.Select(g => g.Key).ToList();
        var products = await _dbContext.Products.Include(p => p.Owner)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();
        ViewData["Products"] = products;
        return View(messages);
    }

    public async Task<IActionResult> Chat(int id)
    {
        var product = await _dbContext.Products.Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        var user = (await _userManager.GetUserAsync(User))!;
        var messages = await _dbContext.Messages.Include(m => m.Sender)
            .Include(m => m.Product)
            .Where(m => m.ProductId == id && (m.BuyerId == user.Id || m.Product.OwnerId == user.Id))
            .ToListAsync();
        ViewData["Product"] = product;
        return View(messages);
    }

    [HttpPost]
    public async Task<IActionResult> SaveImage(int messageId, IFormFile image)
    {
        var url = await _photoManager.SavePhotoAsync(image);
        var message = await _dbContext.Messages.FindAsync(messageId);
        if (message is null)
        {
            return NotFound();
        }

        await _dbContext.SaveChangesAsync();
        return Ok(url);
    }
}