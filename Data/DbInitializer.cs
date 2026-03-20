using Electrical_Equipment_Rental.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Electrical_Equipment_Rental.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        string[] roles = ["Admin", "Manager", "Client", "Technician"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        if (await userManager.FindByEmailAsync("admin@rental.com") is null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@rental.com",
                Email = "admin@rental.com",
                FullName = "Администратор",
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category { Name = "Самокаты", Description = "Электросамокаты для городских поездок", IconClass = "bi-lightning-charge" },
                new Category { Name = "Велосипеды", Description = "Электровелосипеды для прогулок", IconClass = "bi-bicycle" },
                new Category { Name = "Скутеры", Description = "Электроскутеры для дальних поездок", IconClass = "bi-ev-front" },
                new Category { Name = "Гироскутеры", Description = "Гироскутеры и моноколёса", IconClass = "bi-circle" }
            );
            await context.SaveChangesAsync();
        }

        if (!await context.Locations.AnyAsync())
        {
            context.Locations.AddRange(
                new Location { Name = "Центральная точка", Address = "ул. Центральная, 1", Latitude = 47.7631, Longitude = 29.6383 },
                new Location { Name = "Парк Победы", Address = "ул. Парковая, 10", Latitude = 47.7680, Longitude = 29.6450 },
                new Location { Name = "ЖД Вокзал", Address = "пл. Вокзальная, 1", Latitude = 47.7590, Longitude = 29.6320 }
            );
            await context.SaveChangesAsync();
        }

        if (!await context.Products.AnyAsync())
        {
            var categories = await context.Categories.ToListAsync();
            var locations = await context.Locations.ToListAsync();

            var scooterCat = categories.First(c => c.Name == "Самокаты");
            var bikeCat = categories.First(c => c.Name == "Велосипеды");
            var motoScooterCat = categories.First(c => c.Name == "Скутеры");

            var products = new List<Product>
            {
                new Product { Name = "Xiaomi Electric Scooter 4", Description = "Мощный самокат 25 км/ч, запас хода 35 км", PricePerHour = 50, DepositAmount = 500, CategoryId = scooterCat.Id },
                new Product { Name = "Ninebot Max G30", Description = "Премиум самокат, запас хода 65 км", PricePerHour = 80, DepositAmount = 800, CategoryId = scooterCat.Id },
                new Product { Name = "Eltreco XT 600 D", Description = "Горный электровелосипед, 60В, запас хода 80 км", PricePerHour = 100, DepositAmount = 1000, CategoryId = bikeCat.Id },
                new Product { Name = "Volteco Flex UP!", Description = "Городской складной электровелосипед", PricePerHour = 90, DepositAmount = 900, CategoryId = bikeCat.Id },
                new Product { Name = "Citycoco Classic", Description = "Электроскутер 2000W, 60В, до 45 км/ч", PricePerHour = 150, DepositAmount = 2000, CategoryId = motoScooterCat.Id }
            };
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            var loc = locations[0];
            var units = new List<ProductUnit>();
            foreach (var product in products)
            {
                units.Add(new ProductUnit { SerialNumber = $"SN-{product.Id:D3}-001", ProductId = product.Id, Status = UnitStatus.Available, CurrentLocationId = loc.Id });
                units.Add(new ProductUnit { SerialNumber = $"SN-{product.Id:D3}-002", ProductId = product.Id, Status = UnitStatus.Available, CurrentLocationId = loc.Id });
            }
            context.ProductUnits.AddRange(units);
            await context.SaveChangesAsync();
        }
    }
}
