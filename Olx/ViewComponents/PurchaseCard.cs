using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olx.Data;
using Olx.Models;
using Olx.Services.Abstract;
using Olx.ViewModels;

namespace Olx.ViewComponents;

public class PurchaseCard : ViewComponent
{
    private readonly IDeliveryProvider _deliveryProvider;
    private readonly ShopDbContext _context;

    public PurchaseCard(IDeliveryProvider deliveryProvider, ShopDbContext context)
    {
        _deliveryProvider = deliveryProvider;
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(Product product)
    {
        var city = await _deliveryProvider.GetCityByIdAsync(product.City);
        var region = await _deliveryProvider.GetRegionByIdAsync(city.RegionId);
        var vm = new PublicationCardViewModel
        {
            Product = product,
            City = city,
            Region = region,
            IsOwner = product.OwnerId == UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier),
            Order = _context.Orders.FirstOrDefault(o => o.ProductId == product.Id && o.BuyerId == UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier))
        };
        return View(vm);
    }
}