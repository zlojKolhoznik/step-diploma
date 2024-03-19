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
        public IActionResult ByCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _context.Categories.Include(c => c.Filters)
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return Json(GetProperties(category));
        }
        
        private IEnumerable<FilterDeclaration> GetProperties(Category category)
        {
            var properties = new List<FilterDeclaration>();
            properties.AddRange(category.Filters);
            if (category.Parent != null)
            {
                properties.AddRange(GetProperties(category.Parent));
            }
            return properties;
        }
    }
}
