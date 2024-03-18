using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Olx.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey(nameof(Parent))]
    public int? ParentId { get; set; }

    public Category? Parent { get; set; }

    public Category[]? Children { get; set; }

    public Product[]? Products { get; set; }

    [MaxLength(50)]
    public string Name { get; set; }
    
    [MaxLength(50)]
    public string NormalizedName { get; set; }
    
    public List<PropertyDeclaration> Properties { get; set; }
}