using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KovakWeb.Data;
using KovakWeb.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace KovakWeb.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Categorias (Listar categorías del negocio)
        public async Task<IActionResult> Index()
        {
            // Bloqueo de seguridad: Validar sesión activa
            int? negocioId = HttpContext.Session.GetInt32("NegocioID");
            if (negocioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Filtrar estrictamente las categorías del negocio logueado
            var categorias = await _context.Categorias
                .Where(c => c.Activo && c.NegocioID == negocioId.Value)
                .ToListAsync();

            return View(categorias);
        }

        // GET: /Categorias/Crear
        public IActionResult Crear()
        {
            // Bloqueo de seguridad: Validar sesión activa
            if (HttpContext.Session.GetInt32("NegocioID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: /Categorias/Crear
        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            // Bloqueo de seguridad: Validar sesión activa
            int? negocioId = HttpContext.Session.GetInt32("NegocioID");
            if (negocioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Asignar automáticamente el ID del negocio desde la sesión
                    categoria.NegocioID = negocioId.Value;
                    categoria.Activo = true;

                    _context.Categorias.Add(categoria);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(categoria);
        }

        // GET: /Categorias/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null) return NotFound();

            // Bloqueo de seguridad: Validar sesión activa
            int? negocioId = HttpContext.Session.GetInt32("NegocioID");
            if (negocioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Buscar la categoría y validar que pertenezca al negocio de la sesión
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null || categoria.NegocioID != negocioId.Value)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: /Categorias/Editar/5
        [HttpPost]
        public async Task<IActionResult> Editar(int id, Categoria categoria)
        {
            if (id != categoria.ID) return NotFound();

            // Bloqueo de seguridad: Validar sesión activa
            int? negocioId = HttpContext.Session.GetInt32("NegocioID");
            if (negocioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Validar propiedad estricta antes de impactar cambios
            var categoriaOriginal = await _context.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.ID == id);
            if (categoriaOriginal == null || categoriaOriginal.NegocioID != negocioId.Value)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Forzar el negocio correcto desde la sesión por seguridad
                    categoria.NegocioID = negocioId.Value;
                    categoria.Activo = true;

                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(categoria);
        }

        // POST: /Categorias/Ocultar/5
        [HttpPost]
        public async Task<IActionResult> Ocultar(int id)
        {
            // Bloqueo de seguridad: Validar sesión activa
            int? negocioId = HttpContext.Session.GetInt32("NegocioID");
            if (negocioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var categoria = await _context.Categorias.FindAsync(id);

            // Validar que exista y sea del dueño antes de cambiar estado
            if (categoria != null && categoria.NegocioID == negocioId.Value)
            {
                categoria.Activo = !categoria.Activo;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}