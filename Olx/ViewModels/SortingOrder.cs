using System.ComponentModel.DataAnnotations;

namespace Olx.ViewModels;

public enum SortingOrder
{
    [Display(Name="Релевантністю")]
    Relevance,
    [Display(Name="Спочатку нові")]
    NewestFirst,
    [Display(Name="Спочатку дешевші")]
    CheapestFirst,
    [Display(Name="Спочатку дорожчі")]
    CheapestLast
}