using Microsoft.AspNetCore.SignalR;

namespace Olx.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string connectionId, string? messageText, string? imageUrl, string user)
    {
        await Clients.Client(connectionId).SendAsync("ReceiveMessage", messageText, imageUrl, user);
    }
}