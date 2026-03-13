using AutoSalonGrida.Data;
using AutoSalonGrida.Models;
using AutoSalonGrida.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonGrida.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? search, string? category, string? sortOrder)
    {
        IQueryable<Product> query = _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category == category);
        }

        query = sortOrder switch
        {
            "name_desc" => query.OrderByDescending(p => p.Name),
            "date" => query.OrderBy(p => p.CreatedDate),
            "date_desc" => query.OrderByDescending(p => p.CreatedDate),
            _ => query.OrderBy(p => p.Name)
        };

        var categories = await _context.Products
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        var model = new ProductIndexViewModel
        {
            Products = await query.ToListAsync(),
            Categories = new SelectList(categories),
            Search = search,
            Category = category,
            SortOrder = sortOrder
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
        return product is null ? NotFound() : View(product);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new Product { CreatedDate = DateTime.UtcNow });

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        try
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Record created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            ModelState.AddModelError(string.Empty, "Unexpected error while creating the record.");
            return View(product);
        }
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var product = await _context.Products.FindAsync(id);
        return product is null ? NotFound() : View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            return View(product);
        }

        try
        {
            _context.Update(product);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Record updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Products.AnyAsync(e => e.Id == product.Id))
            {
                return NotFound();
            }

            ModelState.AddModelError(string.Empty, "Unable to save changes. Please try again.");
            return View(product);
        }
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
        return product is null ? NotFound() : View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is not null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Record deleted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }
}
