using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // CRÍTICO: Herramienta para cruzar tablas
using KovakWeb.Data;
using KovakWeb.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace KovakWeb.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatalogoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vista pública del celular (Ahora recibe el 'slug' dinámico de la URL)
       [Route("t/{slug}")]
public async Task<IActionResult> Index(string slug, int? categoriaId, string? buscar)
        {
            if (string.IsNullOrEmpty(slug))
            {
                // Si entran a la raíz sin nombre de tienda, los mandamos al login
                return RedirectToAction("Login", "Account");
            }

            // 1. Buscamos el negocio en SQL cuyo nombre coincida con el texto de la URL
            var negocio = await _context.Negocios
                .FirstOrDefaultAsync(n => n.Nombre.ToLower().Replace(" ", "-") == slug.ToLower());

            // Si la tienda no existe en la base de datos, tiramos un error 404 limpio
            if (negocio == null)
            {
                return NotFound();
            }

            // Guardamos el negocio encontrado para usar su Nombre y WhatsApp en la vista
            ViewBag.Negocio = negocio;
            
            // Mandamos las categorías activas que pertenezcan estrictamente a ESTE negocio
            ViewBag.Categorias = await _context.Categorias
                .Where(c => c.Activo && c.NegocioID == negocio.ID)
                .ToListAsync();

            // Iniciamos la consulta de productos ACTIVOS de este negocio con stock
            var query = _context.Productos
                .Include(p => p.ProductoTalles) // <- CRÍTICO: Inyectamos el desglose real de talles desde SQL
                .Where(p => p.Activo && p.Stock > 0 && p.NegocioID == negocio.ID)
                .AsQueryable();

            // Filtro por categoría (si el cliente tocó una)
            if (categoriaId.HasValue)
            {
                query = query.Where(p => p.CategoriaID == categoriaId);
            }

            // Filtro por buscador (si el cliente escribió algo)
            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(p => p.Nombre.Contains(buscar));
            }

            // Ejecutamos la consulta y obtenemos la lista de productos filtrada por negocio
            var productos = await query.ToListAsync();
            return View(productos);
        }
    }
}