using System.ComponentModel.DataAnnotations;

namespace Olx.ViewModels;

public class RegisterViewModel
{
    [Display(Name = "Ім'я")]
    public string Name { get; set; }
    
    [Display(Name = "Електронна пошта")]
    public string Email { get; set; }
    
    [Display(Name = "Створіть пароль")]
    public string Password { get; set; }
    
    [Display(Name = "Підтвердіть пароль")]
    public string ConfirmPassword { get; set; }
    
    
    [Display(Name = "Запам'ятайте мене")]
    public bool IsPersistent { get; set; }
    public string ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }
}