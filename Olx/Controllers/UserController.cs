using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olx.Data;

namespace Olx.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly ShopDbContext _dbContext;

    public UserController(ShopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult FavoriteProducts()
    {
        var products = _dbContext.Products.Include(p => p.FavoredBy);
        var favoriteProducts = products.Where(p => p.FavoredBy!.Any(u => u.UserName == User.Identity!.Name));
        return View(favoriteProducts);
    }
}