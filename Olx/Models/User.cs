using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Olx.Models;

// This class is used to represent the user of the application. In the future, it will propably be replaced by the identity user.
public class User : IdentityUser
{
    public string? Name { get; set; }

    [ForeignKey(nameof(EnterpriseUser))]
    public int? EnterpriseUserId { get; set; }
    
    public EnterpriseUser? EnterpriseUser { get; set; }
}