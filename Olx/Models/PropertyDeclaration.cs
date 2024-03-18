using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Olx.Models;

public class PropertyDeclaration
{
    [Key]
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; }

    [ForeignKey(nameof(Category))]
    public int CategoryId { get; set; }

    public Category Category { get; set; }

    public PropertyType PropertyType { get; set; }

    public List<PropertyValue> PropertyValues { get; set; }
}