using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Olx.Models;

public class FilterValue
{
    [Key]
    public int Id { get; set; }
    
    public string Value { get; set; }  // String representation of the value, it will be converted to the appropriate type in the service layer.
    
    [ForeignKey(nameof(FilterDeclaration))]
    public int FilterDeclarationId { get; set; }

    public FilterDeclaration FilterDeclaration { get; set; }
    
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }
    
    public Product Product { get; set; }
}