using blogic_crm_back.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace blogic_crm_back.Data;

public static class DbInitializer
{
    public static void Seed(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        context.Database.Migrate();

        // Seed roles
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Role { Name = "Admin" },
                new Role { Name = "Advisor" },
                new Role { Name = "Client" }
            );
            context.SaveChanges();
        }

        // Seed test user
        if (!context.Users.Any(u => u.Username == "admin"))
        {
            var adminRole = context.Roles.First(r => r.Name == "Admin");
            var user = new User
            {
                Username = "admin",
                RoleId = adminRole.Id,
                FirstName = "Test",
                LastName = "Admin",
                Email = "admin@example.com"
            };

            user.PasswordHash = passwordHasher.HashPassword(user, "Admin123!");

            context.Users.Add(user);
            context.SaveChanges();
        }
    }
}
