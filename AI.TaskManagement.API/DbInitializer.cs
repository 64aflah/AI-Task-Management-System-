using AI.TaskManagement.Domain.Entities;
using AI.TaskManagement.Infrastructure.Data;
using AI.TaskManagement.Shared.Utilities;

namespace AI.TaskManagement.API;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Migrate the database
        await context.Database.EnsureCreatedAsync();

        // Seed roles if not already seeded
        if (!context.Roles.Any())
        {
            var roles = new[]
            {
                new Role { Name = "Admin", Description = "Administrator role with full access" },
                new Role { Name = "Manager", Description = "Manager role with limited administrative access" },
                new Role { Name = "Member", Description = "Regular member role" }
            };

            context.Roles.AddRange(roles);
            await context.SaveChangesAsync();

            // Seed admin user
            var adminRole = roles.First(r => r.Name == "Admin");
            var adminUser = new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@taskmanagement.com",
                PasswordHash = PasswordHasher.HashPassword("Admin@123456"),
                RoleId = adminRole.Id,
                IsActive = true
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }
    }
}
