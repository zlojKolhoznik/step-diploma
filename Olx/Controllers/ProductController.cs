using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Olx.Data;
using Olx.Models;
using Olx.Services.Abstract;

namespace Olx.Controllers
{
    public class ProductController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly IPhotoManager _photoManager;

        public ProductController(ShopDbContext context, IPhotoManager photoManager)
        {
            _context = context;
            _photoManager = photoManager;
        }

        // GET: Product -- Table of all products for admin panel
        public async Task<IActionResult> Index()
        {
            var shopDbContext = _context.Products.Include(p => p.Category).Include(p => p.Owner);
            return View(await shopDbContext.ToListAsync());
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

        // public async Task<IActionResult> GetProductsByFilters(int? categoryId, int? pricing, int? itemState,
        //     string? city, double? minPrice, double? maxPrice, Dictionary<string, string>? filters)
        // {
        //     var products = _context.Products.AsQueryable();
        //     if (categoryId is not null)
        //     {
        //         products = products.Where(p => p.CategoryId == categoryId);
        //         if (filters is not null)
        //         {
        //             products = await FilterProductsAsync(products, categoryId, filters);
        //         }
        //     }
        //     
        //     if (pricing is not null)
        //     {
        //         var priceType = (PriceType)pricing;
        //         if (Enum.IsDefined(priceType))
        //         {
        //             products = products.Where(p => p.PriceType == priceType);
        //
        //             if (priceType == PriceType.Regular)
        //             {
        //                 products = products.Where(p => p.Price >= (minPrice ?? 0) && p.Price <= (maxPrice ?? double.MaxValue));
        //             }
        //         }
        //         else
        //         {
        //             ModelState.AddModelError("pricing", "Invalid pricing type");
        //         }
        //     }
        //     
        //     if (itemState is not null)
        //     {
        //         if (Enum.IsDefined((ItemState)itemState))
        //         {
        //             products = products.Where(p => p.ItemState == (ItemState)itemState);
        //         }
        //         else
        //         {
        //             ModelState.AddModelError("itemState", "Invalid item state");
        //         }
        //     }
        //     
        //     if (city is not null)
        //     {
        //         products = products.Where(p => p.City == city);
        //     }
        //     
        //     return View(await products.ToListAsync());
        // }
        //
        // private async Task<IQueryable<Product>> FilterProductsAsync(IQueryable<Product> products, int? categoryId, Dictionary<string,string> filters)
        // {
        //     IEnumerable<FilterDeclaration> declarations = await GetFiltersByCategoryAsync(categoryId);
        //     foreach (var declaration in declarations)
        //     {
        //         if (declaration.FilterType == FilterType.Number)
        //         {
        //             if (filters.TryGetValue($"max-{declaration.Name}", out var maxValue))
        //             {
        //                 if (double.TryParse(maxValue, out var max))
        //                 {
        //                     products = products.Where(p => p.Filters.Any(f =>
        //                         f.FilterDeclarationId == declaration.Id && double.Parse(f.Value) <= max));
        //                 }
        //             }
        //             
        //             if (filters.TryGetValue($"min-{declaration.Name}", out var minValue))
        //             {
        //                 if (double.TryParse(minValue, out var min))
        //                 {
        //                     products = products.Where(p => p.Filters.Any(f =>
        //                         f.FilterDeclarationId == declaration.Id && double.Parse(f.Value) >= min));
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             if (filters.TryGetValue(declaration.Name, out var value))
        //             {
        //                 products = products.Where(p => p.Filters.Any(f =>
        //                     f.FilterDeclarationId == declaration.Id && f.Value == value));
        //             }
        //         }
        //
        //     }
        //     return products;
        // }
        //
        private async Task<IEnumerable<FilterDeclaration>> GetFiltersByCategoryAsync(int? categoryId)
        {
            string url = $"https://localhost:7025/Filter/ByCategory/{categoryId}"; // Domain name will probably change so we need to make it dynamic
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

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["PriceType"] = Enum.GetValues<PriceType>().Select(pt => new SelectListItem(pt.ToString(), ((int)pt).ToString()));
            ViewData["ItemState"] = Enum.GetValues<ItemState>().Select(its => new SelectListItem(its.ToString(), ((int)its).ToString()));
            return View();
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
                product.CreatedAt = DateTime.Now;
                product.PhotoUrls = new string[photos.Length];
                for (int i = 0; i < photos.Length; i++)
                {
                    product.PhotoUrls[i] = await _photoManager.SavePhotoAsync(photos[i]);
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
                
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["PriceType"] = Enum.GetValues<PriceType>().Select(pt => new SelectListItem(pt.ToString(), ((int)pt).ToString()));
            ViewData["ItemState"] = Enum.GetValues<ItemState>().Select(its => new SelectListItem(its.ToString(), ((int)its).ToString()));
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["PriceType"] = Enum.GetValues<PriceType>().Select(pt => new SelectListItem(pt.ToString(), ((int)pt).ToString()));
            ViewData["ItemState"] = Enum.GetValues<ItemState>().Select(its => new SelectListItem(its.ToString(), ((int)its).ToString()));
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, string[] leftPhotos, string allPhotos, IFormFile[] photos, IFormCollection form)
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
                    
                    var allPhotoUrls = allPhotos.Split('\n');
                    
                    foreach (var toDelete in allPhotoUrls.Except(leftPhotos))
                    {
                        await _photoManager.DeletePhotoAsync(toDelete);
                    }
                    
                    product.PhotoUrls = new string[photos.Length + leftPhotos.Length];
                    
                    for (int i = 0; i < leftPhotos.Length; i++)
                    {
                        product.PhotoUrls[i] = leftPhotos[i];
                    }
                    
                    for (int i = 0; i < photos.Length; i++)
                    {
                        product.PhotoUrls[i + leftPhotos.Length] = await _photoManager.SavePhotoAsync(photos[i]);
                    }
                    
                    product.UpdatedAt = DateTime.Now;
                    
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["PriceType"] = Enum.GetValues<PriceType>().Select(pt => new SelectListItem(pt.ToString(), ((int)pt).ToString()));
            ViewData["ItemState"] = Enum.GetValues<ItemState>().Select(its => new SelectListItem(its.ToString(), ((int)its).ToString()));
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
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
