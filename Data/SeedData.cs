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

        foreach (var role in new[] { "Администратор", "Пользователь", "Admin", "User" })
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
                FullName = "Главный администратор",
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        if (await context.Cars.AnyAsync()) return;

        var carsData = GetCarsData();
        var cars = carsData.Select((data, idx) => new Car
        {
            Brand = data.Brand,
            Model = data.Model,
            Year = data.Year,
            Price = data.Price,
            CategoryId = data.CategoryId,
            BodyType = data.BodyType,
            Description = data.Description,
            Mileage = data.Mileage,
            EngineType = data.EngineType,
            ImageUrl = $"/images/cars/car-{(idx % 10) + 1}.svg",
            PopularityScore = 100 - idx
        }).ToList();

        await context.Cars.AddRangeAsync(cars);
        await context.SaveChangesAsync();

        var gallery = new List<CarImage>();
        foreach (var car in cars)
        {
            var imageNumber = (car.Id % 10) + 1;
            gallery.Add(new CarImage { CarId = car.Id, ImagePath = $"/images/cars/car-{imageNumber}.svg" });
            gallery.Add(new CarImage { CarId = car.Id, ImagePath = $"/images/cars/car-{((imageNumber + 3) % 10) + 1}.svg" });
        }

        await context.CarImages.AddRangeAsync(gallery);
        await context.SaveChangesAsync();
    }

    private static List<CarSeedItem> GetCarsData() =>
    [
        new("BMW", "X5", 2023, 7800000, 1, "SUV", "3.0 Turbo", 19000, "Комфортный и динамичный кроссовер для города и трассы."),
        new("BMW", "3 Series", 2022, 5300000, 2, "Седан", "2.0 Turbo", 25000, "Легендарный седан с отличной управляемостью."),
        new("Audi", "Q7", 2024, 9200000, 1, "SUV", "3.0 TFSI", 9000, "Семейный SUV премиум-класса."),
        new("Audi", "A6", 2023, 6100000, 2, "Седан", "2.0 TFSI", 17000, "Бизнес-седан с богатым оснащением."),
        new("Mercedes-Benz", "GLE", 2023, 9600000, 1, "SUV", "3.0 Turbo", 14000, "Статусный SUV с высоким уровнем безопасности."),
        new("Mercedes-Benz", "C-Class", 2022, 5600000, 2, "Седан", "2.0 Turbo", 23000, "Классика немецкого премиума."),
        new("Toyota", "Land Cruiser 300", 2024, 10500000, 1, "SUV", "3.5 Twin Turbo", 5000, "Надежный внедорожник для любых условий."),
        new("Toyota", "Camry", 2023, 3900000, 2, "Седан", "2.5", 18000, "Популярный и практичный бизнес-седан."),
        new("Honda", "CR-V", 2022, 4100000, 1, "SUV", "2.0 Hybrid", 27000, "Экономичный и просторный городской SUV."),
        new("Honda", "Civic", 2023, 3200000, 3, "Хэтчбек", "1.5 Turbo", 12000, "Спортивный характер и надежность Honda."),
        new("Ford", "Explorer", 2021, 4300000, 1, "SUV", "2.3 EcoBoost", 36000, "Большой семейный внедорожник."),
        new("Ford", "Mustang", 2022, 6200000, 7, "Купе", "5.0 V8", 21000, "Культовый американский спорткар."),
        new("Chevrolet", "Tahoe", 2022, 7100000, 1, "SUV", "5.3 V8", 28000, "Мощный SUV для дальних поездок."),
        new("Chevrolet", "Malibu", 2021, 2700000, 2, "Седан", "1.5 Turbo", 41000, "Комфортный седан для повседневной езды."),
        new("Tesla", "Model S", 2023, 9800000, 5, "Седан", "Электро", 10000, "Флагманский электромобиль с отличной динамикой."),
        new("Tesla", "Model 3", 2022, 6200000, 5, "Седан", "Электро", 19000, "Технологичный электромобиль для города."),
        new("Nissan", "X-Trail", 2023, 3800000, 1, "SUV", "2.5", 17000, "Практичный кроссовер для семьи."),
        new("Nissan", "Leaf", 2021, 2400000, 5, "Хэтчбек", "Электро", 33000, "Доступный и надежный электромобиль."),
        new("Hyundai", "Santa Fe", 2022, 4100000, 1, "SUV", "2.2 Diesel", 26000, "Современный и просторный SUV."),
        new("Hyundai", "Sonata", 2023, 3400000, 2, "Седан", "2.5", 15000, "Стильный седан с богатой комплектацией."),
        new("Kia", "Sorento", 2023, 4200000, 1, "SUV", "2.5", 16000, "Комфортный кроссовер для всей семьи."),
        new("Kia", "K5", 2022, 3100000, 2, "Седан", "2.0", 23000, "Современный седан со спортивным дизайном."),
        new("Volkswagen", "Touareg", 2022, 6800000, 1, "SUV", "3.0 TDI", 22000, "Немецкий SUV с отличной шумоизоляцией."),
        new("Volkswagen", "Golf", 2023, 2900000, 3, "Хэтчбек", "1.4 TSI", 11000, "Идеальный городской хэтчбек."),
        new("Lexus", "RX", 2023, 7600000, 8, "SUV", "2.5 Hybrid", 14000, "Премиальный комфорт и надежность Lexus."),
        new("Lexus", "ES", 2022, 5900000, 8, "Седан", "2.5", 24000, "Роскошный седан для деловых поездок."),
        new("Mazda", "CX-5", 2023, 3700000, 1, "SUV", "2.5", 13000, "Динамичный дизайн и отличная управляемость."),
        new("Mazda", "6", 2021, 3000000, 2, "Седан", "2.0", 36000, "Элегантный седан с японским качеством."),
        new("Subaru", "Forester", 2022, 3500000, 1, "SUV", "2.0", 29000, "Полный привод и высокий клиренс."),
        new("Subaru", "BRZ", 2023, 3900000, 7, "Купе", "2.4", 9000, "Легкое спортивное купе для драйва."),
        new("Volvo", "XC90", 2023, 8200000, 8, "SUV", "2.0 Hybrid", 12000, "Безопасный премиальный SUV."),
        new("Volvo", "S60", 2022, 4900000, 8, "Седан", "2.0", 22000, "Скандинавский стиль и высокий комфорт."),
        new("Porsche", "Cayenne", 2023, 11800000, 8, "SUV", "3.0 Turbo", 10000, "Премиальный спорт-SUV."),
        new("Porsche", "911 Carrera", 2022, 14500000, 7, "Купе", "3.0 Turbo", 8000, "Икона спортивных автомобилей."),
        new("Ferrari", "F8 Tributo", 2021, 34000000, 7, "Купе", "3.9 V8", 7000, "Суперкар с невероятной динамикой."),
        new("Ferrari", "Roma", 2022, 29500000, 7, "Купе", "3.9 V8", 6000, "Роскошный гран-туризмо Ferrari."),
        new("Lamborghini", "Huracan", 2022, 32000000, 7, "Купе", "5.2 V10", 5000, "Яркий и экстремальный суперкар."),
        new("Lamborghini", "Urus", 2023, 36000000, 8, "SUV", "4.0 V8", 4000, "Самый быстрый люксовый SUV."),
        new("Bentley", "Bentayga", 2022, 28500000, 8, "SUV", "4.0 V8", 11000, "Люксовый внедорожник ручной сборки."),
        new("Bentley", "Flying Spur", 2021, 26000000, 8, "Седан", "6.0 W12", 13000, "Роскошный седан представительского класса."),
        new("Rolls-Royce", "Cullinan", 2023, 52000000, 8, "SUV", "6.75 V12", 3000, "Эталон роскоши среди внедорожников."),
        new("Rolls-Royce", "Ghost", 2022, 47000000, 8, "Седан", "6.75 V12", 4500, "Премиальный седан абсолютного уровня."),
        new("BMW", "i4", 2023, 6900000, 5, "Седан", "Электро", 9000, "Современный электроседан BMW."),
        new("Audi", "e-tron GT", 2023, 11200000, 5, "Купе", "Электро", 7000, "Электроспорткар с премиальным салоном."),
        new("Mercedes-Benz", "EQE", 2024, 9800000, 5, "Седан", "Электро", 2000, "Инновационный электроседан Mercedes."),
        new("Toyota", "Prius", 2022, 3400000, 6, "Хэтчбек", "1.8 Hybrid", 21000, "Легендарный гибрид для города."),
        new("Honda", "Accord Hybrid", 2023, 3700000, 6, "Седан", "2.0 Hybrid", 12000, "Экономичный гибридный седан."),
        new("Hyundai", "Ioniq 5", 2023, 5200000, 5, "SUV", "Электро", 11000, "Футуристичный электрокроссовер."),
        new("Kia", "EV6", 2023, 5400000, 5, "Купе", "Электро", 9000, "Динамичный электрический кроссовер-купе."),
        new("Volkswagen", "ID.4", 2022, 4700000, 5, "SUV", "Электро", 15000, "Практичный электромобиль для семьи."),
        new("Nissan", "Qashqai", 2023, 3300000, 1, "SUV", "1.3 Turbo", 13000, "Популярный городской кроссовер.")
    ];

    private sealed record CarSeedItem(
        string Brand,
        string Model,
        int Year,
        decimal Price,
        int CategoryId,
        string BodyType,
        string EngineType,
        int Mileage,
        string Description);
}
