using backend.Enum;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public static class DbSeeder
{
    public static async Task Seed(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var adminEmail = configuration["AdminSettings:Email"];
            var adminPassword = configuration["AdminSettings:Password"];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                return;
            }

            var existeAdmin = await context.Usuarios
                .AnyAsync(u => u.Rol == ERol.Admin.ToString());

            if (!existeAdmin)
            {
                var adminUsuario = new UsuarioEntity
                {
                    Email = adminEmail,
                    Password_Hash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    Rol = ERol.Admin.ToString(),
                    Avatar_Url = "/uploads/avatars/default-user.png",
                    RefreshToken = string.Empty,
                    RefreshTokenExpiry = DateTime.UtcNow
                };

                context.Usuarios.Add(adminUsuario);
                await context.SaveChangesAsync();
                
            }
        }
    }
}