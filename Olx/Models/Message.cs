using System.ComponentModel.DataAnnotations;

namespace Olx.Models;

public class Message
{
    [Key]
    public int Id { get; set; }
    
    public string? Text { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public int ProductId { get; set; }
    
    public Product Product { get; set; }
    
    public string ReceiverId { get; set; }
    
    public User Receiver { get; set; }
    
    public string SenderId { get; set; }
    
    public User Sender { get; set; }
    
    public bool IsRead { get; set; }
    
    public bool IsDelivered { get; set; }
    
    public DateTime CreatedAt { get; set; }
}