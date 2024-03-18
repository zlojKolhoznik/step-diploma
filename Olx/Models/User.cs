using System.ComponentModel.DataAnnotations;

namespace Olx.Models;

// This class is used to represent the user of the application. In the future, it will propably be replaced by the identity user.
public class User
{
    [Key] 
    public string Id { get; set; } = Guid.NewGuid().ToString();
}