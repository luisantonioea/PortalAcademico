using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

namespace PortalAcademico.Controllers
{
    // El atributo [Authorize] asegura que solo usuarios logueados puedan entrar aquí
    [Authorize] 
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MatriculasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Inscribir(int cursoId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            // 1. Obtener el curso
            var curso = await _context.Cursos.FindAsync(cursoId);
            if (curso == null || !curso.Activo)
            {
                TempData["Error"] = "El curso no existe o no está disponible.";
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }

            // 2. Validar que no esté ya matriculado (Regla P1)
            bool yaMatriculado = await _context.Matriculas
                .AnyAsync(m => m.CursoId == cursoId && m.UsuarioId == userId && m.Estado != EstadoMatricula.Cancelada);
            
            if (yaMatriculado)
            {
                TempData["Error"] = "Ya te encuentras matriculado en este curso.";
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }

            // 3. Validar Cupo Máximo
            int inscritosActuales = await _context.Matriculas
                .CountAsync(m => m.CursoId == cursoId && m.Estado != EstadoMatricula.Cancelada);
            
            if (inscritosActuales >= curso.CupoMaximo)
            {
                TempData["Error"] = "Lo sentimos, el curso ya alcanzó su cupo máximo.";
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }

            // 4. Validar Solapamiento de Horarios
            var misCursos = await _context.Matriculas
                .Include(m => m.Curso)
                .Where(m => m.UsuarioId == userId && m.Estado != EstadoMatricula.Cancelada)
                .Select(m => m.Curso)
                .ToListAsync();

            // Lógica de cruce de horarios: (InicioA < FinB) y (FinA > InicioB)
            bool hayCruce = misCursos.Any(c => 
                curso.HorarioInicio < c.HorarioFin && curso.HorarioFin > c.HorarioInicio);

            if (hayCruce)
            {
                TempData["Error"] = "El horario de este curso se cruza con otro en el que ya estás inscrito.";
                return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
            }

            // 5. Crear la Matrícula en estado Pendiente
            var nuevaMatricula = new Matricula
            {
                CursoId = cursoId,
                UsuarioId = userId,
                Estado = EstadoMatricula.Pendiente
            };

            _context.Matriculas.Add(nuevaMatricula);
            await _context.SaveChangesAsync();

            TempData["Exito"] = "¡Inscripción exitosa! Tu matrícula está en estado Pendiente.";
            return RedirectToAction("Detalle", "Cursos", new { id = cursoId });
        }
    }
}