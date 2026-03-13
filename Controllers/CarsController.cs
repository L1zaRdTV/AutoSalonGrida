using AutoSalonGrida.Data;
using AutoSalonGrida.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonGrida.Controllers;

[AllowAnonymous]
public class CarsController : Controller
{
    private readonly ApplicationDbContext _context;

    public CarsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search, string? brand, int? categoryId, decimal? minPrice, decimal? maxPrice, int? minYear, int? maxYear, string? sort)
    {
        var query = _context.Cars.AsNoTracking().Include(c => c.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Brand.Contains(search) || c.Model.Contains(search));

        if (!string.IsNullOrWhiteSpace(brand)) query = query.Where(c => c.Brand == brand);
        if (categoryId.HasValue) query = query.Where(c => c.CategoryId == categoryId.Value);
        if (minPrice.HasValue) query = query.Where(c => c.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(c => c.Price <= maxPrice.Value);
        if (minYear.HasValue) query = query.Where(c => c.Year >= minYear.Value);
        if (maxYear.HasValue) query = query.Where(c => c.Year <= maxYear.Value);

        query = sort switch
        {
            "price_desc" => query.OrderByDescending(c => c.Price),
            "year_asc" => query.OrderBy(c => c.Year),
            "year_desc" => query.OrderByDescending(c => c.Year),
            _ => query.OrderBy(c => c.Price)
        };

        var brands = await _context.Cars.AsNoTracking().Select(c => c.Brand).Distinct().OrderBy(b => b).ToListAsync();

        return View(new CarCatalogViewModel
        {
            Cars = await query.ToListAsync(),
            Categories = await _context.Categories.AsNoTracking().OrderBy(c => c.Name).Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToListAsync(),
            Brands = brands.Select(b => new SelectListItem(b, b)).ToList(),
            Search = search,
            Brand = brand,
            CategoryId = categoryId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            MinYear = minYear,
            MaxYear = maxYear,
            Sort = sort
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var car = await _context.Cars.Include(c => c.Category).FirstOrDefaultAsync(c => c.Id == id);
        if (car is null) return NotFound();

        var gallery = await _context.CarImages.Where(i => i.CarId == id).ToListAsync();
        if (gallery.Count == 0) gallery.Add(new Models.CarImage { ImagePath = car.ImageUrl });

        return View(new CarDetailsViewModel { Car = car, Gallery = gallery });
    }

    public async Task<IActionResult> Categories() => View(await _context.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync());

    public async Task<IActionResult> Brands() => View(await _context.Cars.AsNoTracking().Select(c => c.Brand).Distinct().OrderBy(b => b).ToListAsync());
}
