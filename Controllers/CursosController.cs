using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.ViewModels;

namespace PortalAcademico.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Cursos
        public async Task<IActionResult> Index(CatalogoViewModel model)
        {
            // Consulta base: solo cursos activos
            var query = _context.Cursos.Where(c => c.Activo).AsQueryable();

            if (!ModelState.IsValid)
            {
                // Si la validación falla (ej. créditos negativos), devolvemos la lista sin filtrar
                model.Cursos = await query.ToListAsync();
                return View(model);
            }

            // Aplicar filtros dinámicamente
            if (!string.IsNullOrEmpty(model.BuscarNombre))
            {
                query = query.Where(c => c.Nombre.Contains(model.BuscarNombre));
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

            model.Cursos = await query.ToListAsync();
            return View(model);
        }

        // GET: /Cursos/Detalle/5
        public async Task<IActionResult> Detalle(int id)
        {
            var curso = await _context.Cursos.FirstOrDefaultAsync(c => c.Id == id && c.Activo);
            if (curso == null) return NotFound();

            return View(curso);
        }
    }
}