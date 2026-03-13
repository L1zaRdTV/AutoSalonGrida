using AutoSalonGrida.Data;
using AutoSalonGrida.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonGrida.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index() => View(await _context.Roles.ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var role = await _context.Roles.FirstOrDefaultAsync(m => m.IdRole == id);
            if (role == null) return NotFound();
            return View(role);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRole,Name")] Role role)
        {
            if (ModelState.IsValid)
            {
                _context.Add(role);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Роль успешно добавлена.";
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRole,Name")] Role role)
        {
            if (id != role.IdRole) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(role);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Роль успешно обновлена.";
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var role = await _context.Roles.FirstOrDefaultAsync(m => m.IdRole == id);
            if (role == null) return NotFound();

            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Роль удалена.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
