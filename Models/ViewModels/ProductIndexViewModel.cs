using AutoSalonGrida.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoSalonGrida.Models.ViewModels;

public class ProductIndexViewModel
{
    public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
    public SelectList Categories { get; set; } = new(Enumerable.Empty<string>());

    public string? Search { get; set; }
    public string? Category { get; set; }
    public string? SortOrder { get; set; }
}
