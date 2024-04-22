using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Olx.Models;

public class User : IdentityUser
{
    public string? Name { get; set; }

    public int? EnterpriseUserId { get; set; }
    
    public EnterpriseUser? EnterpriseUser { get; set; }
    
    public List<Message> Messages { get; set; }
}