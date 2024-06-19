using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olx.Data;
using Olx.Models;
using Olx.Services.Abstract;
using Olx.ViewModels;

namespace Olx.ViewComponents;

public class OwnProductCard : ViewComponent
{
    private readonly IDeliveryProvider _deliveryProvider;
    private readonly ShopDbContext _context;

    public OwnProductCard(IDeliveryProvider deliveryProvider, ShopDbContext context)
    {
        _deliveryProvider = deliveryProvider;
        _context = context;
    }
    
    public async Task<IViewComponentResult> InvokeAsync(Product product)
    {
        var tmp = await _deliveryProvider.GetRegionsAsync();
        var city = await _deliveryProvider.GetCityByIdAsync(product.City);
        var region = await _deliveryProvider.GetRegionByIdAsync(city.RegionId);
        var likes = _context.Users.Include(u => u.Favorites)
            .Count(u => u.Favorites.Any(f => f.Id == product.Id));
        var chats = _context.Users.Include(u => u.Messages)
            .Count(u => u.Messages.Any(m => m.ProductId == product.Id));
        var vm = new OwnProductCardViewModel
        {
            Product = product,
            City = city,
            Region = region,
            Likes = likes,
            Chats = chats
        };
        return View(vm);
    }
}