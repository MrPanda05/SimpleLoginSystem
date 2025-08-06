using SimpleLoginSystem.Objects;
using Microsoft.EntityFrameworkCore;

namespace SimpleLoginSystem
{
    public static class DataSeeder
    {
        public static async Task SeedAdminUserAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<EcommerceDB>();

                    // Ensure the database is created
                    await context.Database.EnsureCreatedAsync();

                    // Check if any user already exists
                    if (!await context.Users.AnyAsync(u => u.Role == "admin"))
                    {
                        Console.WriteLine("--> Seeding new Admin user...");

                        var adminUser = new User
                        {
                            Username = "admin",
                            Email = "admin@mail.com",
                            Role = "admin",
                            Password = "admin123",
                        };

                        await context.Users.AddAsync(adminUser);
                        await context.SaveChangesAsync();

                        Console.WriteLine("--> Admin user seeded successfully.");
                    }
                    else
                    {
                        Console.WriteLine("--> Admin user already exists. No seeding needed.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred during seeding: {ex.Message}");
                }
            }
        }
    }
}
