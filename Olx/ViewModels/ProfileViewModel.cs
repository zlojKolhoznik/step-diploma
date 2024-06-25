using Olx.Models;

namespace Olx.ViewModels;

public class ProfileViewModel
{
    public User User { get; set; }

    public int ActivePublications { get; set; }

    public int HiddenPublications { get; set; }

    public int ArchivedPublications { get; set; }
    
    public int RejectedPublications { get; set; }

    public int Sells { get; set; }
    
    public int Purchases { get; set; }
}