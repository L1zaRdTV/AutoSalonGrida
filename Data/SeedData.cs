using AutoSalonGrida.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonGrida.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        string[] roles = ["Admin", "User"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        const string adminEmail = "admin@autosalon.local";
        const string adminPassword = "Admin123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        if (!await context.Cars.AnyAsync())
        {
            context.Cars.AddRange(
                new Car
                {
                    Brand = "BMW", Model = "X5", Year = 2024, Price = 69900, CategoryId = 1, BodyType = "SUV", EngineType = "3.0 Turbo",
                    Mileage = 12000, ImageUrl = "/images/cars/bmw-x5-main.svg", PopularityScore = 95,
                    Description = "Премиальный SUV с интеллектуальным полным приводом и цифровым кокпитом."
                },
                new Car
                {
                    Brand = "Tesla", Model = "Model S", Year = 2023, Price = 81900, CategoryId = 5, BodyType = "Sedan", EngineType = "Electric",
                    Mileage = 8000, ImageUrl = "/images/cars/tesla-models-main.svg", PopularityScore = 99,
                    Description = "Электромобиль с впечатляющей динамикой, автопилотом и премиальной отделкой."
                },
                new Car
                {
                    Brand = "Audi", Model = "A5 Coupe", Year = 2022, Price = 53400, CategoryId = 4, BodyType = "Coupe", EngineType = "2.0 TFSI",
                    Mileage = 24000, ImageUrl = "/images/cars/audi-a5-main.svg", PopularityScore = 89,
                    Description = "Спортивное купе с выразительным дизайном и комфортом для ежедневных поездок."
                }
            );
            await context.SaveChangesAsync();

            var tesla = await context.Cars.FirstAsync(c => c.Brand == "Tesla");
            context.CarImages.AddRange(
                new CarImage { CarId = tesla.Id, ImagePath = "/images/cars/tesla-models-main.svg" },
                new CarImage { CarId = tesla.Id, ImagePath = "/images/cars/tesla-models-side.svg" },
                new CarImage { CarId = tesla.Id, ImagePath = "/images/cars/tesla-models-back.svg" }
            );
            await context.SaveChangesAsync();
        }
    }
}
