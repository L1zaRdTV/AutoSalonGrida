using AutoSalonGrida.Data;
using AutoSalonGrida.Models;
using AutoSalonGrida.Services;
using AutoSalonGrida.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AutoSalonGrida.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var hash = PasswordHasher.Hash(model.Password);
            var user = await _context.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == model.Login && u.PasswordHash == hash);

            if (user is null || user.Role is null)
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Role, user.Role.Name),
                new("Login", user.Login)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            TempData["Success"] = "Вы успешно вошли в систему.";
            return RedirectToAction("Index", "Users");
        }

        [AllowAnonymous]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existing = await _context.Users.AnyAsync(u => u.Login == model.Login);
            if (existing)
            {
                ModelState.AddModelError(nameof(model.Login), "Пользователь с таким логином уже существует.");
                return View(model);
            }

            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Пользователь");
            if (userRole is null)
            {
                ModelState.AddModelError(string.Empty, "Роль 'Пользователь' не найдена в системе.");
                return View(model);
            }

            var user = new User
            {
                Name = model.Name,
                Age = model.Age,
                Login = model.Login,
                PasswordHash = PasswordHasher.Hash(model.Password),
                IdRole = userRole.IdRole
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Регистрация прошла успешно. Теперь войдите в систему.";
            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Вы вышли из системы.";
            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied() => View();
    }
}
