using Microsoft.AspNetCore.Identity;
using ThriftStoreWebApp.Models;

namespace ThriftStoreWebApp.Services
{
    public static class DatabaseInitializer
    {
        public static async Task SeedDataAsync(
            UserManager<ApplicationUser>? userManager,
            RoleManager<IdentityRole>? roleManager)
        {
            if (userManager == null || roleManager == null)
            {
                Console.WriteLine("[SeedData] UserManager or RoleManager is null. Seeding aborted.");
                return;
            }

            // Ensure roles exist
            foreach (var role in new[] { "admin", "seller", "client" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    Console.WriteLine($"[SeedData] '{role}' role not found. Creating...");
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Ensure at least one admin user exists
            if ((await userManager.GetUsersInRoleAsync("admin")).Any())
            {
                Console.WriteLine("[SeedData] Admin user already exists.");
                return;
            }

            var adminUser = new ApplicationUser
            {
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                CreatedAt = DateTime.Now
            };

            const string initialPassword = "admin123";

            var createResult = await userManager.CreateAsync(adminUser, initialPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "admin");

                Console.WriteLine("[SeedData] Admin user created successfully!");
                Console.WriteLine($"Email: {adminUser.Email}");
                Console.WriteLine($"Initial Password: {initialPassword}");
            }
            else
            {
                Console.WriteLine("[SeedData] Failed to create admin user.");
                foreach (var error in createResult.Errors)
                {
                    Console.WriteLine($"- {error.Description}");
                }
            }
        }
    }
}
