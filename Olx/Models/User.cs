using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Olx.Models;

public class User : IdentityUser
{
    public string? Name { get; set; }

    public int? EnterpriseUserId { get; set; }
    
    public EnterpriseUser? EnterpriseUser { get; set; }
    
    public List<Message> Messages { get; set; }

    public DateOnly RegistrationDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    
    [ValidateNever]
    public List<Product>? Favorites { get; set; }
    
    [ValidateNever]
    public List<Product>? Products { get; set; }
    
    [ValidateNever]
    public List<Order>? Orders { get; set; }

    [Range(0, double.MaxValue)]
    public double Balance { get; set; } = 0;
    
    public string? ProfilePictureUrl { get; set; }
}