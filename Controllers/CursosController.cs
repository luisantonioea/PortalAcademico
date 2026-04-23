using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PortalAcademico.Data;
using PortalAcademico.Models;
using PortalAcademico.ViewModels;
using System.Text.Json;

namespace PortalAcademico.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache; // Inyectamos la caché

        public CursosController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: /Cursos
        public async Task<IActionResult> Index(CatalogoViewModel model)
        {
            string cacheKey = "CursosActivosList";
            List<Curso> cursosActivos;

            // 1. Intentar obtener los cursos desde la Caché
            var cachedCursos = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedCursos))
            {
                // Si existe en caché, lo deserializamos
                cursosActivos = JsonSerializer.Deserialize<List<Curso>>(cachedCursos) ?? new List<Curso>();
            }
            else
            {
                // Si no existe (o expiraron los 60s), vamos a la base de datos
                cursosActivos = await _context.Cursos.Where(c => c.Activo).ToListAsync();

                // Guardar en caché por exactamente 60 segundos
                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cursosActivos), cacheOptions);
            }

            // 2. Aplicar los filtros sobre la lista en memoria
            var query = cursosActivos.AsQueryable();

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.BuscarNombre))
                {
                    query = query.Where(c => c.Nombre.Contains(model.BuscarNombre, StringComparison.OrdinalIgnoreCase));
                }
                if (model.MinCreditos.HasValue)
                {
                    query = query.Where(c => c.Creditos >= model.MinCreditos.Value);
                }
                if (model.MaxCreditos.HasValue)
                {
                    query = query.Where(c => c.Creditos <= model.MaxCreditos.Value);
                }
                if (model.HorarioInicio.HasValue)
                {
                    query = query.Where(c => c.HorarioInicio >= model.HorarioInicio.Value);
                }
                if (model.HorarioFin.HasValue)
                {
                    query = query.Where(c => c.HorarioFin <= model.HorarioFin.Value);
                }
            }

            model.Cursos = query.ToList();
            return View(model);
        }

        // GET: /Cursos/Detalle/5
        public async Task<IActionResult> Detalle(int id)
        {
            var curso = await _context.Cursos.FirstOrDefaultAsync(c => c.Id == id && c.Activo);
            if (curso == null) return NotFound();

            // Guardar en Sesión el último curso visitado (Id y Nombre)
            HttpContext.Session.SetString("UltimoCursoId", curso.Id.ToString());
            HttpContext.Session.SetString("UltimoCursoNombre", curso.Nombre);

            return View(curso);
        }
    }
}