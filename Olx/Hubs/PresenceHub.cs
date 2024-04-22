using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Olx.Models;

namespace Olx.Hubs;

[Authorize]
public class PresenceHub : Hub
{
    private readonly UserManager<User> _userManager;

    public PresenceHub(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
}