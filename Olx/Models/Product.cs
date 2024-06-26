﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Olx.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [MinLength(5), MaxLength(50)]
    public string Name { get; set; }

    [MinLength(20), MaxLength(1_000)]
    public string Description { get; set; }

    [ForeignKey(nameof(Category))]
    public int CategoryId { get; set; }

    [ValidateNever]
    public Category Category { get; set; }

    [ValidateNever]
    public string[] PhotoUrls { get; set; }

    public double? Price { get; set; }
    
    public ItemState ItemState { get; set; }
    
    [ValidateNever]
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? RejectedAt { get; set; }
    
    public bool IsRejected => RejectedAt is not null && (UpdatedAt is null || RejectedAt > UpdatedAt);
    
    [MinLength(5), MaxLength(500)]
    public string? RejectionReason { get; set; }

    public bool IsAutorenewing { get; set; }
    
    public bool IsPromoted { get; set; }

    public DateTime? PromotionEnd { get; set; }

    [Required(ErrorMessage = "Оберіть місто зі списку")]
    public string City { get; set; } // City REF from Nova Poshta API

    [ForeignKey(nameof(Owner))]
    public string OwnerId { get; set; }
    
    [ValidateNever]
    public User Owner { get; set; }
    
    [ValidateNever]
    public List<FilterValue> Filters { get; set; }

    public List<Message>? Messages { get; set; }
    
    [ValidateNever]
    public List<User>? FavoredBy { get; set; }
    
    public List<Order>? Orders { get; set; }

    [Range(0, int.MaxValue)]
    public int QuantityAvailable { get; set; } = 1;

    public PublicationState PublicationState { get; set; }
}