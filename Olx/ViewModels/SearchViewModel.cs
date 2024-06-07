using Olx.Models;

namespace Olx.ViewModels;

public class SearchViewModel
{
    public string? Query { get; set; }
    public string? City { get; set; }
    public string? MinPrice { get; set; }
    public string? MaxPrice { get; set; }
    public string[]? ItemState { get; set; }
    
    public string? OrderBy { get; set; }
    public string? Category { get; set; }
    public List<Product>? Results { get; set; }
    
    public List<Category>? Categories { get; set; }
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; }

    public int Count { get; set; }
}