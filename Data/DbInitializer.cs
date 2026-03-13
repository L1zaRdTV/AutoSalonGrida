using AutoSalonGrida.Models;
using AutoSalonGrida.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonGrida.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(
                    new Role { Name = "Администратор" },
                    new Role { Name = "Пользователь" });
                await context.SaveChangesAsync();
            }

            if (!await context.Users.AnyAsync())
            {
                var adminRoleId = await context.Roles.Where(r => r.Name == "Администратор").Select(r => r.IdRole).FirstAsync();
                var userRoleId = await context.Roles.Where(r => r.Name == "Пользователь").Select(r => r.IdRole).FirstAsync();

                context.Users.AddRange(
                    new User { Name = "Главный администратор", Age = 35, Login = "admin", PasswordHash = PasswordHasher.Hash("admin123"), IdRole = adminRoleId },
                    new User { Name = "Иван Петров", Age = 26, Login = "ivanp", PasswordHash = PasswordHasher.Hash("user123"), IdRole = userRoleId },
                    new User { Name = "Мария Соколова", Age = 31, Login = "maria.s", PasswordHash = PasswordHasher.Hash("user123"), IdRole = userRoleId },
                    new User { Name = "Олег Смирнов", Age = 28, Login = "oleg-sm", PasswordHash = PasswordHasher.Hash("user123"), IdRole = userRoleId }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
