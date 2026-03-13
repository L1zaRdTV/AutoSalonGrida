using AutoSalonGrida.Data;
using AutoSalonGrida.Models;
using AutoSalonGrida.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonGrida.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AdminController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var cars = await _context.Cars.Include(c => c.Category).OrderByDescending(c => c.Id).ToListAsync();
        return View(cars);
    }

    public async Task<IActionResult> Create()
    {
        return View(await BuildCarFormViewModelAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CarFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.CategoryOptions = await CategoryOptionsAsync();
            return View(model);
        }

        var mainImage = await SaveImageAsync(model.MainImage);
        var car = new Car
        {
            Brand = model.Brand,
            Model = model.Model,
            Year = model.Year,
            Price = model.Price,
            CategoryId = model.CategoryId,
            BodyType = model.BodyType,
            Description = model.Description,
            Mileage = model.Mileage,
            EngineType = model.EngineType,
            ImageUrl = mainImage ?? model.ImageUrl ?? "/images/cars/default-car.svg"
        };

        _context.Cars.Add(car);
        await _context.SaveChangesAsync();

        await SaveGalleryImagesAsync(car.Id, model.GalleryImages);
        TempData["Success"] = "Автомобиль добавлен.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car is null) return NotFound();

        return View(await BuildCarFormViewModelAsync(car));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CarFormViewModel model)
    {
        if (id != model.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            model.CategoryOptions = await CategoryOptionsAsync();
            return View(model);
        }

        var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id);
        if (car is null) return NotFound();

        car.Brand = model.Brand;
        car.Model = model.Model;
        car.Year = model.Year;
        car.Price = model.Price;
        car.CategoryId = model.CategoryId;
        car.BodyType = model.BodyType;
        car.Description = model.Description;
        car.Mileage = model.Mileage;
        car.EngineType = model.EngineType;

        var mainImage = await SaveImageAsync(model.MainImage);
        car.ImageUrl = mainImage ?? model.ImageUrl ?? car.ImageUrl;

        await SaveGalleryImagesAsync(car.Id, model.GalleryImages);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Автомобиль обновлен.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var car = await _context.Cars.Include(c => c.Category).FirstOrDefaultAsync(c => c.Id == id);
        return car is null ? NotFound() : View(car);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car is not null)
        {
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Categories() => View(await _context.Categories.OrderBy(c => c.Name).ToListAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(string name)
    {
        if (!string.IsNullOrWhiteSpace(name) && !await _context.Categories.AnyAsync(c => c.Name == name))
        {
            _context.Categories.Add(new Category { Name = name });
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Categories));
    }

    private async Task<CarFormViewModel> BuildCarFormViewModelAsync(Car? car = null)
    {
        return new CarFormViewModel
        {
            Id = car?.Id,
            Brand = car?.Brand ?? string.Empty,
            Model = car?.Model ?? string.Empty,
            Year = car?.Year ?? DateTime.UtcNow.Year,
            Price = car?.Price ?? 0,
            CategoryId = car?.CategoryId ?? 1,
            BodyType = car?.BodyType ?? "SUV",
            Description = car?.Description ?? string.Empty,
            Mileage = car?.Mileage ?? 0,
            EngineType = car?.EngineType ?? string.Empty,
            ImageUrl = car?.ImageUrl,
            CategoryOptions = await CategoryOptionsAsync()
        };
    }

    private async Task<List<SelectListItem>> CategoryOptionsAsync() => await _context.Categories
        .OrderBy(c => c.Name)
        .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
        .ToListAsync();

    private async Task<string?> SaveImageAsync(IFormFile? image)
    {
        if (image is null || image.Length == 0) return null;

        var folder = Path.Combine(_environment.WebRootPath, "images", "cars");
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
        var path = Path.Combine(folder, fileName);

        await using var stream = System.IO.File.Create(path);
        await image.CopyToAsync(stream);
        return $"/images/cars/{fileName}";
    }

    private async Task SaveGalleryImagesAsync(int carId, IEnumerable<IFormFile>? images)
    {
        if (images is null) return;

        foreach (var image in images.Where(i => i.Length > 0))
        {
            var path = await SaveImageAsync(image);
            if (path is not null)
            {
                _context.CarImages.Add(new CarImage { CarId = carId, ImagePath = path });
            }
        }

        await _context.SaveChangesAsync();
    }
}
