using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Olx.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    
    public int? ParentId { get; set; }

    [ForeignKey(nameof(ParentId))]
    public Category? Parent { get; set; }

    public Category[]? Children { get; set; }

    public string Name { get; set; }
    
    public string NormalizedName { get; set; }
    
    // TODO: Ask how to implement custom filters for each category.
    // public Filters { get; set; }
}