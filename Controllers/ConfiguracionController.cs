using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KovakWeb.Data;
using KovakWeb.Models;

namespace KovakWeb.Controllers
{
    public class ConfiguracionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConfiguracionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Buscamos el primer negocio que exista
            var negocio = await _context.Negocios.FirstOrDefaultAsync();
            
            // Si la base de datos está vacía, lo creamos automáticamente
            if (negocio == null)
            {
                negocio = new Negocio 
                { 
                    Nombre = "Mi Catálogo", 
                    WhatsApp = "3810000000",
                    UrlPersonalizada = "mi-catalogo" // <- ACÁ ESTÁ LA SOLUCIÓN
                };
                _context.Negocios.Add(negocio);
                await _context.SaveChangesAsync();
            }

            return View(negocio);
        }

        // Guarda los cambios del WhatsApp y Nombre
        [HttpPost]
        public async Task<IActionResult> Guardar(Negocio negocio)
        {
            if (ModelState.IsValid)
            {
                _context.Update(negocio);
                await _context.SaveChangesAsync();
                TempData["MensajeExito"] = "Los datos se actualizaron correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View("Index", negocio);
        }
    }
}