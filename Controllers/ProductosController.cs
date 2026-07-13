using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KovakWeb.Data;
using KovakWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace KovakWeb.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Listar stock en el panel
        public async Task<IActionResult> Index()
        {
            // Bloqueo de seguridad: Validar sesión activa
            int? negocioId = HttpContext.Session.GetInt32("NegocioID");
            if (negocioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Filtrar estrictamente por el negocio que inició sesión
            var productos = await _context.Productos
                .Where(p => p.Activo && p.NegocioID == negocioId.Value)
                .ToListAsync();

            return View(productos);
        }

        // GET: Crear
        public IActionResult Crear()
        {
            // Bloqueo de seguridad: Validar sesión activa
            int? negocioId = HttpContext.Session.GetInt32("NegocioID");
            if (negocioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Cargar solo las categorías pertenecientes a este negocio
            ViewBag.Categorias = _context.Categorias
                .Where(c => c.Activo && c.NegocioID == negocioId.Value)
                .ToList();

            return View();
        }

        // POST: Crear con desglose de talles
        [HttpPost]
        public async Task<IActionResult> Crear(Producto producto, IFormFile? img1, IFormFile? img2, IFormFile? img3, IFormFile? img4, string[] talles, int[] cantidades)
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
                    // Asignar dinámicamente el ID del negocio desde la sesión
                    producto.NegocioID = negocioId.Value;

                    // Guardar imágenes físicas
                    producto.UrlImagen = await GuardarImagenFisica(img1);
                    producto.UrlImagen2 = await GuardarImagenFisica(img2);
                    producto.UrlImagen3 = await GuardarImagenFisica(img3);
                    producto.UrlImagen4 = await GuardarImagenFisica(img4);

                    // El stock general será la suma de todas las cantidades por talle
                    producto.Stock = cantidades != null ? cantidades.Sum() : 0;

                    _context.Productos.Add(producto);
                    await _context.SaveChangesAsync(); // Se genera el producto.ID

                    // Guardar el desglose en la tabla ProductoTalles
                    if (talles != null && cantidades != null)
                    {
                        for (int i = 0; i < talles.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(talles[i]))
                            {
                                var pt = new ProductoTalle
                                {
                                    ProductoID = producto.ID,
                                    Talle = talles[i].Trim().ToUpper(),
                                    Cantidad = cantidades[i]
                                };
                                _context.ProductoTalles.Add(pt);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // Si algo falla, recargar las categorías de este negocio antes de re-renderizar la vista
            ViewBag.Categorias = _context.Categorias
                .Where(c => c.Activo && c.NegocioID == negocioId.Value)
                .ToList();

            return View(producto);
        }

        // GET: Editar
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null) return NotFound();

            // Bloqueo de seguridad: Validar sesión activa
            int? negocioId = HttpContext.Session.GetInt32("NegocioID");
            if (negocioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Buscar el producto y validar de forma cruzada que pertenezca al negocio de la sesión
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null || producto.NegocioID != negocioId.Value) 
            {
                return NotFound();
            }

            // Cargar solo las categorías pertenecientes a este negocio
            ViewBag.Categorias = await _context.Categorias
                .Where(c => c.Activo && c.NegocioID == negocioId.Value)
                .ToListAsync();
            
            // Enviamos los talles actuales cargados a la vista
            ViewBag.TallesActuales = await _context.ProductoTalles
                .Where(pt => pt.ProductoID == id)
                .ToListAsync();
            
            return View(producto);
        }

        // POST: Editar con actualización de talles
        [HttpPost]
        public async Task<IActionResult> Editar(int id, Producto producto, IFormFile? img1, IFormFile? img2, IFormFile? img3, IFormFile? img4, string[] talles, int[] cantidades)
        {
            if (id != producto.ID) return NotFound();

            // Bloqueo de seguridad: Validar sesión activa
            int? negocioId = HttpContext.Session.GetInt32("NegocioID");
            if (negocioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Validación de propiedad estricta antes de guardar
            var productoOriginal = await _context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.ID == id);
            if (productoOriginal == null || productoOriginal.NegocioID != negocioId.Value)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Conservar el NegocioID original e intacto desde el flujo de sesión
                    producto.NegocioID = negocioId.Value;

                    producto.UrlImagen = await GuardarImagenFisica(img1) ?? producto.UrlImagen;
                    producto.UrlImagen2 = await GuardarImagenFisica(img2) ?? producto.UrlImagen2;
                    producto.UrlImagen3 = await GuardarImagenFisica(img3) ?? producto.UrlImagen3;
                    producto.UrlImagen4 = await GuardarImagenFisica(img4) ?? producto.UrlImagen4;

                    producto.Stock = cantidades != null ? cantidades.Sum() : 0;
                    _context.Update(producto);

                    // Limpieza rigurosa: Borramos los talles viejos antes de insertar los nuevos mapeados
                    var tallesViejos = _context.ProductoTalles.Where(pt => pt.ProductoID == id);
                    _context.ProductoTalles.RemoveRange(tallesViejos);

                    if (talles != null && cantidades != null)
                    {
                        for (int i = 0; i < talles.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(talles[i]))
                            {
                                var pt = new ProductoTalle
                                {
                                    ProductoID = id,
                                    Talle = talles[i].Trim().ToUpper(),
                                    Cantidad = cantidades[i]
                                };
                                _context.ProductoTalles.Add(pt);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // Si hay error en el formulario, recargar datos encapsulados por negocio
            ViewBag.Categorias = _context.Categorias
                .Where(c => c.Activo && c.NegocioID == negocioId.Value)
                .ToList();

            ViewBag.TallesActuales = await _context.ProductoTalles
                .Where(pt => pt.ProductoID == id)
                .ToListAsync();

            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Ocultar(int id)
        {
            // Bloqueo de seguridad: Validar sesión activa
            int? negocioId = HttpContext.Session.GetInt32("NegocioID");
            if (negocioId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var producto = await _context.Productos.FindAsync(id);
            
            // Validar que el producto exista y pertenezca al negocio logueado antes de mutar el estado
            if (producto != null && producto.NegocioID == negocioId.Value)
            {
                producto.Activo = !producto.Activo;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> GuardarImagenFisica(IFormFile? archivo)
        {
            if (archivo == null || archivo.Length == 0) return null;
            if (archivo.Length > 2 * 1024 * 1024) throw new Exception("Una imagen supera los 2MB permitidos.");

            string rutaCarpeta = Path.Combine(_env.WebRootPath, "images", "productos");
            if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

            string nombreArchivoUnico = Guid.NewGuid().ToString() + "_" + archivo.FileName;
            string rutaFisicaCompleta = Path.Combine(rutaCarpeta, nombreArchivoUnico);

            using (var fileStream = new FileStream(rutaFisicaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(fileStream);
            }

            return "/images/productos/" + nombreArchivoUnico;
        }
    }
}