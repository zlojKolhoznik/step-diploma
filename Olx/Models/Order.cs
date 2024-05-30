using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Olx.Models;

public class Order
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }

    [ValidateNever]
    public Product Product { get; set; }
    
    public string BuyerId { get; set; }
    
    [ValidateNever]
    public User Buyer { get; set; }

    public bool IsPaymentCompleted { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public OrderState State { get; set; } = OrderState.Pending;
    
    public DeliveryMethod DeliveryMethod { get; set; }

    // Delivery details
    
    public string ReceiverName { get; set; }

    public string ReceiverLastName { get; set; }

    public string? ReceiverPatronymyc { get; set; }
    
    public string ReceiverPhoneNumber { get; set; }
    
    public string ReceiverEmail { get; set; }

    public string ReceiverCity { get; set; }
    
    // Properties below are optional since they depend on the delivery type

    public string? ReceiverWarehouse { get; set; }

    public string? ReceiverAddress { get; set; }

    public string? ReceiverZipCode { get; set; }
}