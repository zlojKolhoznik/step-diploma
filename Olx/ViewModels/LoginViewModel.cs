using System.ComponentModel.DataAnnotations;

namespace Olx.ViewModels;

public class LoginViewModel
{
    [Display(Name = "Електронна пошта")]
    public string Username { get; set; }

    [Display(Name = "Пароль")]
    public string Password { get; set; }
    
    [Display(Name = "Запам'ятати мене")]
    public bool IsPersistent { get; set; }
    
    public string? ReturnUrl { get; set; }
    
    public string? ErrorMessage { get; set; }
}