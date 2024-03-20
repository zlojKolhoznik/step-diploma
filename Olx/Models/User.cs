using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Olx.Models;

public class User : IdentityUser
{
    public string? Name { get; set; }

    [ForeignKey(nameof(EnterpriseUser))]
    public int? EnterpriseUserId { get; set; }
    
    public EnterpriseUser? EnterpriseUser { get; set; }
}