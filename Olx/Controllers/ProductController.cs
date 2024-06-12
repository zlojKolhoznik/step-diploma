using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Olx.Data;
using Olx.Models;
using Olx.Services.Abstract;

namespace Olx.Controllers;

public class ProductController : Controller
{
    private readonly ShopDbContext _context;
    private readonly IPhotoManager _photoManager;
    private readonly UserManager<User> _userManager;

    public ProductController(ShopDbContext context, IPhotoManager photoManager, UserManager<User> userManager)
    {
        _context = context;
        _photoManager = photoManager;
        _userManager = userManager;
    }

    // GET: Product -- Table of all products for admin panel
    public async Task<IActionResult> Index()
    {
        var products = _context.Products.Include(p => p.Category).Include(p => p.Owner);
        return View(await products.ToListAsync());
    }

    // GET: Product/Details/5 -- Details of a single product for admin panel
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }
    
    private async Task<IEnumerable<FilterDeclaration>> GetFiltersByCategoryAsync(int? categoryId)
    {
        string url =
            $"https://localhost:7025/Filter/ByCategory/{categoryId}"; // Domain name will probably change so we need to make it dynamic
        using var client = new HttpClient();
        client.BaseAddress = new Uri(url);
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to get filters");
        }

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<IEnumerable<FilterDeclaration>>(content);
        return result ?? throw new Exception("Failed to deserialize filters");

    }

    // POST: Product/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormCollection form, IFormFile[] photos)
    {
        if (photos.Length == 0)
        {
            ModelState.AddModelError("PhotoUrls", "At least one photo is required");
        }

        if (ModelState.IsValid)
        {
            var time = DateTime.Now;
            product.CreatedAt = time;
            product.UpdatedAt = time;
            product.PhotoUrls = new string[photos.Length];
            for (int i = 0; i < photos.Length; i++)
            {
                string url = await _photoManager.SavePhotoAsync(photos[i]);
                product.PhotoUrls[i] = "/" + url.Replace("\\", "/");
            }

            var filterDeclarations = await GetFiltersByCategoryAsync(product.CategoryId);
            foreach (var filterDeclaration in filterDeclarations)
            {
                if (form.TryGetValue(filterDeclaration.Name, out var value))
                {
                    await _context.FilterValues.AddAsync(new FilterValue
                    {
                        FilterDeclarationId = filterDeclaration.Id,
                        Product = product,
                        Value = value.ToString()
                    });
                }
            }

            var user = await _userManager.GetUserAsync(User);
            var enteredPhone = new string(form["sellerPhone"].ToString().Where(char.IsDigit).ToArray());
            user!.PhoneNumber = user.PhoneNumber is not null
                ? new string(user.PhoneNumber.Where(char.IsDigit).ToArray())
                : enteredPhone;
            _context.Users.Update(user);
            product.PublicationState = PublicationState.Active;
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        ViewData["PriceType"] = Enum.GetValues<PriceType>()
            .Select(pt => new SelectListItem(pt.ToString(), ((int)pt).ToString()));
        ViewData["ItemState"] = Enum.GetValues<ItemState>()
            .Select(its => new SelectListItem(its.ToString(), ((int)its).ToString()));
        return View(product);
    }

    // GET: Product/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var product = await _context.Products.Include(p => p.Filters)
            .ThenInclude(f => f.FilterDeclaration)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        ViewData["PriceType"] = Enum.GetValues<PriceType>()
            .Select(pt => new SelectListItem(pt.ToString(), ((int)pt).ToString()));
        ViewData["ItemState"] = Enum.GetValues<ItemState>()
            .Select(its => new SelectListItem(its.ToString(), ((int)its).ToString()));
        return View(product);
    }

    [Authorize]
    public async Task<IActionResult> Create()
    {
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        ViewData["PriceType"] = Enum.GetValues<PriceType>()
            .Select(pt => new SelectListItem(pt.ToString(), ((int)pt).ToString()));
        ViewData["ItemState"] = Enum.GetValues<ItemState>()
            .Select(its => new SelectListItem(its.ToString(), ((int)its).ToString()));
        return View(new Product());
    }

    // POST: Product/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product, string[] leftPhotos, string allPhotos,
        IFormFile[] photos, IFormCollection form)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (photos.Length + leftPhotos.Length == 0)
        {
            ModelState.AddModelError("PhotoUrls", "At least one photo is required");
        }

        if (ModelState.IsValid)
        {
            try
            {
                var filterValues = await _context.FilterValues
                    .Include(fv => fv.FilterDeclaration)
                    .Where(fv => fv.ProductId == product.Id)
                    .ToListAsync();

                foreach (var filterValue in filterValues)
                {
                    if (!form.TryGetValue(filterValue.FilterDeclaration.Name, out var value))
                    {
                        continue;
                    }

                    filterValue.Value = value.ToString();
                    _context.Update(filterValue);
                }

                var allPhotoUrls = allPhotos.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

                foreach (var toDelete in allPhotoUrls.Except(leftPhotos))
                {
                    await _photoManager.DeletePhotoAsync(toDelete[1..]);
                }

                product.PhotoUrls = new string[photos.Length + leftPhotos.Length];

                for (int i = 0; i < leftPhotos.Length; i++)
                {
                    product.PhotoUrls[i] = leftPhotos[i];
                }

                for (int i = 0; i < photos.Length; i++)
                {
                    string url = await _photoManager.SavePhotoAsync(photos[i]);
                    product.PhotoUrls[i + leftPhotos.Length] = "/" + url.Replace("\\", "/");
                }

                product.UpdatedAt = DateTime.Now;

                var user = await _userManager.GetUserAsync(User);
                var enteredPhone = new string(form["sellerPhone"].ToString().Where(char.IsDigit).ToArray());
                if (user.PhoneNumber is null ||
                    new string(user.PhoneNumber.Where(char.IsDigit).ToArray()) != enteredPhone)
                {
                    user.PhoneNumber = enteredPhone;
                    await _userManager.UpdateAsync(user);
                }

                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        product.PhotoUrls = allPhotos.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        ViewData["PriceType"] = Enum.GetValues<PriceType>()
            .Select(pt => new SelectListItem(pt.ToString(), ((int)pt).ToString()));
        ViewData["ItemState"] = Enum.GetValues<ItemState>()
            .Select(its => new SelectListItem(its.ToString(), ((int)its).ToString()));
        return View(product);
    }

    // GET: Product/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }

    // POST: Product/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is not null)
        {
            foreach (var photoUrl in product.PhotoUrls)
            {
                await _photoManager.DeletePhotoAsync(photoUrl[1..]);
            }

            _context.Products.Remove(product);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Product/5
    // Product details for users placeholder
    public async Task<IActionResult> UserView(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }
    
    // GET: Product/5
    // Product card
    [Route("Product/{id:int}")]
    public async Task<IActionResult> Card(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .Include(p => p.Category)
            .ThenInclude(c => c.Parent)
            .Include(p => p.Owner)
            .ThenInclude(u => u.Products)
            .Include(p => p.Filters)
            .ThenInclude(f => f.FilterDeclaration).Include(product => product.FavoredBy)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product is null)
        {
            return NotFound();
        }
        
        List<Category> categoryChain = [];
        var category = product.Category;
        while (category is not null)
        {
            categoryChain.Add(category);
            category = category.Parent;
        }
        
        var suggestedProducts = await _context.Products
            .Include(p => p.Owner)
            .Include(p => p.FavoredBy)
            .Where(p => p.Id != product.Id)
            .OrderByDescending(p => Guid.NewGuid())
            .Take(6)
            .ToListAsync();
        
        ViewData["SuggestedProducts"] = suggestedProducts;
        ViewData["CategoryChain"] = categoryChain;
        ViewData["IsFavorite"] = User.Identity.IsAuthenticated 
                                 && product.FavoredBy is not null 
                                 && product.FavoredBy.Any(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

        return View(product);
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ToggleFavorite(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var product = await _context.Products.FindAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users.Include(u => u.Favorites)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return NotFound();
        }

        if (!user.Favorites.Remove(product))
        {
            user.Favorites.Add(product);
        }

        await _context.SaveChangesAsync();
        return Ok(new { productId = id, isFavorite = user.Favorites.Contains(product) });
    }

    [Authorize]
    public async Task<IActionResult> ClearFavorites()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users.Include(u => u.Favorites)
            .FirstAsync(u => u.Id == userId);
        user.Favorites!.Clear();
        await _context.SaveChangesAsync();
        return RedirectToAction("FavoriteProducts", "User");
    }
}