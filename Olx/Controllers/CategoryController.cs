using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Olx.Data;
using Olx.Models;
using Olx.Services.Abstract;

namespace Olx.Controllers;

public class CategoryController : Controller
{
    private readonly ShopDbContext _context;
    private readonly IPhotoManager _photoManager;

    public CategoryController(ShopDbContext context, IPhotoManager photoManager)
    {
        _context = context;
        _photoManager = photoManager;
    }

    // GET: Category
    public async Task<IActionResult> Index()
    {
        var shopDbContext = _context.Categories.Include(c => c.Parent);
        return View(await shopDbContext.ToListAsync());
    }

    // GET: Category/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .Include(c => c.Parent)
            .Include(c => c.Children)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // GET: Category/Create
    public IActionResult Create()
    {
        ViewData["ParentId"] = new SelectList(_context.Categories, "Id", "Name");
        ViewData["FilterTypes"] = Enum.GetValues<FilterType>()
            .Select(v => new SelectListItem(v.ToString(), ((int)v).ToString()));
        return View();
    }

    // POST: Category/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,ParentId,Name,NormalizedName")] Category category,
        string[] filters, int[] types, IFormFile icon)
    {
        if (filters.SkipLast(1).Any(string.IsNullOrWhiteSpace)) // last element may be empty due to the way the form is rendered
        {
            ModelState.AddModelError("Filters", "Please provide a name for each filter.");
        }

        if (string.IsNullOrWhiteSpace(category.NormalizedName))
        {
            category.NormalizedName = string.Join('-', category.Name.ToLower().Split(' '));
        }

        if (ModelState.IsValid)
        {
            var iconUrl = await _photoManager.SavePhotoAsync(icon);
            category.IconUrl = iconUrl;
            _context.Add(category);
            await _context.SaveChangesAsync();
            if (!string.IsNullOrWhiteSpace(filters[0]))
            {
                for (int i = 0; i < filters.Length; i++)
                {
                    _context.FilterDeclarations.Add(new FilterDeclaration
                    {
                        CategoryId = category.Id,
                        Name = filters[i],
                        FilterType = (FilterType)types[i]
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["Categories"] = new SelectList(_context.Categories, "Id", "Id", category.ParentId);
        ViewData["FilterTypes"] = Enum.GetValues<FilterType>()
            .Select(v => new SelectListItem(v.ToString(), ((int)v).ToString()));
        return View(category);
    }

    // GET: Category/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .Include(c => c.Parent)
            .Include(c => c.Filters)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        ViewData["FilterTypes"] = Enum.GetValues<FilterType>()
            .Select(v => new SelectListItem(v.ToString(), ((int)v).ToString()));
        ViewData["Categories"] = new SelectList(_context.Categories.Where(c => c.Id != id), "Id", "Name", category.ParentId);
        return View(category);
    }

    // POST: Category/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,ParentId,Name,NormalizedName")] Category category,
        string[] filters, int[] types, int[] filterIds, IFormFile? icon)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (icon is not null)
                {
                    if (!string.IsNullOrWhiteSpace(category.IconUrl))
                    {
                        await _photoManager.DeletePhotoAsync(category.IconUrl[1..]);
                    }
                    var newUrl = await _photoManager.SavePhotoAsync(icon);
                    category.IconUrl = newUrl;
                }
                _context.Update(category);
                int i = 0;
                for (; i < filterIds.Length; i++)
                {
                    var filter = await _context.FilterDeclarations.FindAsync(filterIds[i]);
                    filter.Name = filters[i];
                    filter.FilterType = (FilterType)types[i];
                    _context.Update(filter);
                }
                
                for (; i < filters.Length; i++)
                {
                    _context.FilterDeclarations.Add(new FilterDeclaration
                    {
                        CategoryId = category.Id,
                        Name = filters[i],
                        FilterType = (FilterType)types[i]
                    });
                }

                foreach (var filterDeclaration in _context.FilterDeclarations.Where(fd => fd.CategoryId == category.Id && !filterIds.Contains(fd.Id)))
                {
                    _context.FilterDeclarations.Remove(filterDeclaration);
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["Categories"] = new SelectList(_context.Categories.Where(c => c.Id != category.Id), "Id", "Name", category.ParentId);
        return View(category);
    }

    // GET: Category/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .Include(c => c.Parent)
            .Include(c => c.Children)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category is null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: Category/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is not null)
        {
            if (!string.IsNullOrWhiteSpace(category.IconUrl))
            {
                await _photoManager.DeletePhotoAsync(category.IconUrl[1..]);
            }
            _context.Categories.Remove(category);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}