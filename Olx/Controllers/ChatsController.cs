using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Olx.Data;
using Olx.Hubs;
using Olx.Models;
using Olx.Services.Abstract;

namespace Olx.Controllers;

[Authorize]
public class ChatsController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ShopDbContext _dbContext;
    private readonly IPhotoManager _photoManager;
    private readonly IHubContext<ChatHub> _chatHub;

    public ChatsController(UserManager<User> userManager, ShopDbContext dbContext, IPhotoManager photoManager, IHubContext<ChatHub> chatHub)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _photoManager = photoManager;
        _chatHub = chatHub;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var user = (await _userManager.GetUserAsync(User))!;
        var messages = await _dbContext.Messages.Include(m => m.Product)
            .Include(m => m.Receiver)
            .Include(m => m.Sender)
            .Where(m => m.SenderId == user.Id || m.ReceiverId == user.Id)
            .GroupBy(m => m.ProductId)
            .ToListAsync();
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
            .Where(m => m.ProductId == id && (m.ReceiverId == user.Id || m.SenderId == user.Id))
            .ToListAsync();
        ViewData["Product"] = product;
        messages.Where(m => !m.IsRead && m.ReceiverId == user.Id)
            .ToList()
            .ForEach(async m =>
            {
                m.IsRead = true;
                await _chatHub.Clients.User(m.SenderId).SendAsync("MessageRead", m.Id);
            });
        await _dbContext.SaveChangesAsync();
        return View(messages);
    }

    // id - product id
    [HttpPost]
    public async Task<IActionResult> SendMessage(int? id, string? message, string? senderId, string? receiverId, IFormFile? image)
    {
        if (id is null)
        {
            return NotFound();
        }
        
        var product = await _dbContext.Products.Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }
        
        if (senderId is null || receiverId is null)
        {
            return BadRequest("Both senderId and receiverId must be provided");
        }

        string? imageUrl = null;
        if (image is not null)
        {
            imageUrl = await _photoManager.SavePhotoAsync(image);
        }
        
        var messageEntity = new Message
        {
            ProductId = id.Value,
            ReceiverId = receiverId,
            SenderId = senderId,
            Text = message,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.Now
        };
        
        await _dbContext.Messages.AddAsync(messageEntity);
        await _dbContext.SaveChangesAsync();
        return Ok(new {userId = receiverId, messageId = messageEntity.Id, imageUrl = imageUrl});
    }

    public async Task<IActionResult> GetMessage(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }
        
        var message = (await _dbContext.Messages.Include(m => m.Sender)
            .ToListAsync())
            .FirstOrDefault(m => m.Id == id);
        if (message is null)
        {
            return NotFound();
        }
        
        return Ok(message);
    }
}