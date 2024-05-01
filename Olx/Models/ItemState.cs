using System.ComponentModel.DataAnnotations;

namespace Olx.Models;

public enum ItemState
{
    [Display(Name = "Нове")]
    New,
    [Display(Name = "Вживане")]
    Used
}