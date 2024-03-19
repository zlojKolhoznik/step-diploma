using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Olx.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey(nameof(Parent))]
    [Display(Name = "Parent Category")]
    public int? ParentId { get; set; }

    public Category? Parent { get; set; }

    public List<Category>? Children { get; set; }

    public List<Product>? Products { get; set; }

    [MaxLength(50)]
    public string Name { get; set; }
    
    [MaxLength(50)]
    [ValidateNever]
    [Display(Name = "Normalized Name")]
    public string NormalizedName { get; set; }
    
    [ValidateNever]
    public List<FilterDeclaration> Filters { get; set; }
}