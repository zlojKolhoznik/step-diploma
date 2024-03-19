using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Olx.Models;

public class FilterDeclaration
{
    [Key]
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; }

    [ForeignKey(nameof(Category))]
    public int CategoryId { get; set; }

    public Category Category { get; set; }

    public FilterType FilterType { get; set; }

    public List<FilterValue> FilterValues { get; set; }
}