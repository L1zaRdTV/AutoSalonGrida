using AutoSalonGrida.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoSalonGrida.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CarImage> CarImages => Set<CarImage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Car>()
            .HasOne(c => c.Category)
            .WithMany(c => c.Cars)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CarImage>()
            .HasOne(ci => ci.Car)
            .WithMany(c => c.Images)
            .HasForeignKey(ci => ci.CarId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "SUV" },
            new Category { Id = 2, Name = "Sedan" },
            new Category { Id = 3, Name = "Hatchback" },
            new Category { Id = 4, Name = "Coupe" },
            new Category { Id = 5, Name = "Electric" },
            new Category { Id = 6, Name = "Hybrid" },
            new Category { Id = 7, Name = "Luxury" },
            new Category { Id = 8, Name = "Sport" }
        );
    }
}
