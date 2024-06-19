using Olx.Models;

namespace Olx.ViewModels;

public class OwnProductCardViewModel
{
    public Product Product { get; set; }
    
    public City City { get; set; }
    
    public Region Region { get; set; }

    public int Likes { get; set; }
    
    public int Chats { get; set; }
}