using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers
{
    [Authorize(Roles = "Coordinador")]
    public class CoordinadorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public CoordinadorController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // --- CRUD DE CURSOS ---

        // GET: /Coordinador
        public async Task<IActionResult> Index()
        {
            var cursos = await _context.Cursos.ToListAsync();
            return View(cursos);
        }

        // GET: /Coordinador/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: /Coordinador/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Curso curso)
        {
            if (ModelState.IsValid)
            {
                _context.Cursos.Add(curso);
                await _context.SaveChangesAsync();
                
                // Invalidar caché al crear
                await _cache.RemoveAsync("CursosActivosList");

                TempData["Exito"] = "Curso creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        // GET: /Coordinador/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        // POST: /Coordinador/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Curso curso)
        {
            if (id != curso.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(curso);
                await _context.SaveChangesAsync();
                
                // Invalidar caché al editar
                await _cache.RemoveAsync("CursosActivosList");

                TempData["Exito"] = "Curso actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        // POST: /Coordinador/Desactivar/5
        [HttpPost]
        public async Task<IActionResult> Desactivar(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso != null)
            {
                curso.Activo = !curso.Activo;
                await _context.SaveChangesAsync();
                
                // Invalidar caché al cambiar estado
                await _cache.RemoveAsync("CursosActivosList"); 
            }
            return RedirectToAction(nameof(Index));
        }

        // --- GESTIÓN DE MATRÍCULAS ---

        // GET: /Coordinador/Matriculas/5
        public async Task<IActionResult> Matriculas(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            ViewBag.CursoNombre = curso.Nombre;

            var matriculas = await _context.Matriculas
                .Include(m => m.Usuario)
                .Where(m => m.CursoId == id)
                .ToListAsync();

            return View(matriculas);
        }

        // POST: /Coordinador/CambiarEstadoMatricula
        [HttpPost]
        public async Task<IActionResult> CambiarEstadoMatricula(int matriculaId, EstadoMatricula estado)
        {
            var matricula = await _context.Matriculas.FindAsync(matriculaId);
            if (matricula != null)
            {
                matricula.Estado = estado;
                await _context.SaveChangesAsync();
                TempData["Exito"] = $"Matrícula actualizada a {estado}.";
                return RedirectToAction(nameof(Matriculas), new { id = matricula.CursoId });
            }
            return NotFound();
        }
    }
}