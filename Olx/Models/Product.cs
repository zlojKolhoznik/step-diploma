using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Olx.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [MinLength(16), MaxLength(50)]
    public string Name { get; set; }

    [MinLength(40), MaxLength(10_000)]
    public string Description { get; set; }

    [ForeignKey(nameof(Category))]
    public int CategoryId { get; set; }

    public Category Category { get; set; }

    public string[] PhotoUrls { get; set; }

    public PriceType PriceType { get; set; }

    public double? Price { get; set; }

    public SellerType SellerType { get; set; }

    public ItemState ItemState { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }

    public bool IsAutorenewing { get; set; }

    public string City { get; set; } // This is a temporary solution. We will use either a separate table or some sort of API to get the city name.

    [ForeignKey(nameof(Owner))]
    public string OwnerId { get; set; }
    
    public User Owner { get; set; }

    public string ContactPhoneNumber { get; set; }

    public List<PropertyValue> Properties { get; set; }
}