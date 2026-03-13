using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoSalonGrida.Models.ViewModels;

public class CarCatalogViewModel
{
    public List<Car> Cars { get; set; } = [];
    public List<SelectListItem> Categories { get; set; } = [];
    public List<SelectListItem> Brands { get; set; } = [];
    public string? Search { get; set; }
    public string? Brand { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinYear { get; set; }
    public int? MaxYear { get; set; }
    public string? BodyType { get; set; }
    public string? Sort { get; set; }
}
