using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Models;

namespace PortalAcademico.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.Migrate();

            if (!await roleManager.RoleExistsAsync("Coordinador"))
            {
                await roleManager.CreateAsync(new IdentityRole("Coordinador"));
            }

            if (await userManager.FindByEmailAsync("admin@portal.com") == null)
            {
                var user = new IdentityUser { UserName = "admin@portal.com", Email = "admin@portal.com", EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Coordinador");
                }
            }

            if (!context.Cursos.Any())
            {
                context.Cursos.AddRange(
                    new Curso { Codigo = "CS101", Nombre = "Introducción a .NET", Creditos = 4, CupoMaximo = 30, HorarioInicio = new TimeSpan(8, 0, 0), HorarioFin = new TimeSpan(10, 0, 0), Activo = true },
                    new Curso { Codigo = "CS102", Nombre = "Bases de Datos", Creditos = 3, CupoMaximo = 25, HorarioInicio = new TimeSpan(10, 0, 0), HorarioFin = new TimeSpan(12, 0, 0), Activo = true },
                    new Curso { Codigo = "CS103", Nombre = "Arquitectura de Software", Creditos = 5, CupoMaximo = 20, HorarioInicio = new TimeSpan(14, 0, 0), HorarioFin = new TimeSpan(16, 0, 0), Activo = true }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}