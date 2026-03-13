using AutoSalonGrida.Data;
using AutoSalonGrida.Models;
using AutoSalonGrida.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonGrida.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int? roleId, int? minAge, int? maxAge, string? sortOrder)
        {
            ViewData["CurrentSearch"] = search;
            ViewData["CurrentRoleId"] = roleId;
            ViewData["CurrentMinAge"] = minAge;
            ViewData["CurrentMaxAge"] = maxAge;
            ViewData["CurrentSort"] = sortOrder;

            ViewData["RoleId"] = new SelectList(await _context.Roles.ToListAsync(), "IdRole", "Name", roleId);
            ViewData["NameSort"] = sortOrder == "name_desc" ? "name_asc" : "name_desc";
            ViewData["AgeSort"] = sortOrder == "age_desc" ? "age_asc" : "age_desc";
            ViewData["RoleSort"] = sortOrder == "role_desc" ? "role_asc" : "role_desc";

            IQueryable<User> query = _context.Users.Include(u => u.Role);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.Name.Contains(search) || u.Login.Contains(search));
            }

            if (roleId.HasValue)
            {
                query = query.Where(u => u.IdRole == roleId.Value);
            }

            if (minAge.HasValue)
            {
                query = query.Where(u => u.Age >= minAge.Value);
            }

            if (maxAge.HasValue)
            {
                query = query.Where(u => u.Age <= maxAge.Value);
            }

            query = sortOrder switch
            {
                "name_asc" => query.OrderBy(u => u.Name),
                "name_desc" => query.OrderByDescending(u => u.Name),
                "age_asc" => query.OrderBy(u => u.Age),
                "age_desc" => query.OrderByDescending(u => u.Age),
                "role_asc" => query.OrderBy(u => u.Role!.Name),
                "role_desc" => query.OrderByDescending(u => u.Role!.Name),
                _ => query.OrderBy(u => u.IdUser)
            };

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(m => m.IdUser == id);
            if (user == null) return NotFound();

            return View(user);
        }

        [Authorize(Roles = "Администратор")]
        public IActionResult Create()
        {
            ViewData["IdRole"] = new SelectList(_context.Roles, "IdRole", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Create([Bind("IdUser,Name,Age,Login,PasswordHash,IdRole")] User user)
        {
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                ModelState.AddModelError(nameof(user.PasswordHash), "Укажите пароль.");
            }

            if (await _context.Users.AnyAsync(u => u.Login == user.Login))
            {
                ModelState.AddModelError(nameof(user.Login), "Логин уже используется.");
            }

            if (ModelState.IsValid)
            {
                user.PasswordHash = PasswordHasher.Hash(user.PasswordHash);
                _context.Add(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Запись успешно добавлена.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdRole"] = new SelectList(_context.Roles, "IdRole", "Name", user.IdRole);
            return View(user);
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.PasswordHash = string.Empty;

            ViewData["IdRole"] = new SelectList(_context.Roles, "IdRole", "Name", user.IdRole);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Edit(int id, [Bind("IdUser,Name,Age,Login,PasswordHash,IdRole")] User formUser)
        {
            if (id != formUser.IdUser) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.IdUser == id);
            if (user == null) return NotFound();

            if (await _context.Users.AnyAsync(u => u.Login == formUser.Login && u.IdUser != formUser.IdUser))
            {
                ModelState.AddModelError(nameof(formUser.Login), "Логин уже используется.");
            }

            if (ModelState.IsValid)
            {
                user.Name = formUser.Name;
                user.Age = formUser.Age;
                user.Login = formUser.Login;
                user.IdRole = formUser.IdRole;

                if (!string.IsNullOrWhiteSpace(formUser.PasswordHash))
                {
                    user.PasswordHash = PasswordHasher.Hash(formUser.PasswordHash);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Запись успешно обновлена.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdRole"] = new SelectList(_context.Roles, "IdRole", "Name", formUser.IdRole);
            return View(formUser);
        }

        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(m => m.IdUser == id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Запись удалена.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
