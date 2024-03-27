using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olx.Data;
using Olx.Models;

namespace Olx.Controllers
{
    public class FilterController : Controller
    {
        private readonly ShopDbContext _context;

        public FilterController(ShopDbContext context)
        {
            _context = context;
        }
        
        // GET: Filter/ByCategory/5
        // This is a API call, it returns a JSON object with all the properties of the category with the given id.
        public async Task<IActionResult> ByCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _context.Categories.Include(c => c.Filters)
                .FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            var filterDeclarations = await GetFiltersAsync(category);
            foreach (var filterDeclaration in filterDeclarations)
            {
                filterDeclaration.Category = null;
            }
            return Json(filterDeclarations);
        }
        
        private async Task<List<FilterDeclaration>> GetFiltersAsync(Category category)
        {
            var filters = new List<FilterDeclaration>();
            filters.AddRange(category.Filters);
            var parent = await _context.Categories.Include(c => c.Filters).FirstOrDefaultAsync(c => c.Id == category.ParentId);
            if (parent is not null)
            {
                filters.AddRange(await GetFiltersAsync(parent));
            }
            return filters.ToList();
        }
    }
}
