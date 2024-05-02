using System.ComponentModel.DataAnnotations;

namespace Olx.Models;

public enum PriceType
{
    [Display(Name = "Фіксована ціна")]
    Regular,
    [Display(Name = "Безкоштовно")]
    Free,
    [Display(Name = "Договірна ціна")]
    Contract,
    [Display(Name = "Обмін")]
    Barter
}