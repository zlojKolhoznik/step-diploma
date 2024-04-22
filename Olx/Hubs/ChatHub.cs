using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Olx.Data;
using Olx.Models;

namespace Olx.Hubs;

public class ChatHub : Hub
{
    private readonly UserManager<User> _userManager;
    private readonly ShopDbContext _dbContext;

    public ChatHub(UserManager<User> userManager, ShopDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task SendMessage(string userId, int messageId)
    {
        await Clients.User(userId).SendAsync("ReceiveMessage", messageId);
    }
    
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("UserConnected", Context.ConnectionId, _userManager.GetUserId(Context.User));
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task MarkAsRead(int messageId)
    {
        var message = await _dbContext.Messages.FindAsync(messageId);
        if (message is null)
        {
            return;
        }

        message.IsRead = true;
        await _dbContext.SaveChangesAsync();
        await Clients.User(message.SenderId).SendAsync("MessageRead", messageId);
    }
}