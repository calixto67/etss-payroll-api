using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PayrollApi.Domain.Entities;

namespace PayrollApi.Infrastructure.Data;

/// <summary>
/// Seeds default data on startup in Development. Safe to run multiple times (idempotent).
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        // ── Admin user ────────────────────────────────────────────────────────
        if (!await context.Users.AnyAsync(u => u.Username == "admin"))
        {
            context.Users.Add(new User
            {
                Username  = "admin",
                Email     = "admin@etss.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234"),
                Role      = "Admin",
                IsActive  = true,
                CreatedBy = "system",
                CreatedAt = DateTime.UtcNow,
            });

            await context.SaveChangesAsync();
            logger.LogInformation("Seeded default admin user — admin@etss.com / Admin@1234");
        }
    }
}
