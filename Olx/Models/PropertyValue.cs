using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Olx.Models;

public class PropertyValue
{
    [Key]
    public int Id { get; set; }
    
    public string Value { get; set; }  // String representation of the value, it will be converted to the appropriate type in the service layer.
    
    [ForeignKey(nameof(PropertyDeclaration))]
    public int PropertyDeclarationId { get; set; }

    public PropertyDeclaration PropertyDeclaration { get; set; }
    
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }
    
    public Product Product { get; set; }
}