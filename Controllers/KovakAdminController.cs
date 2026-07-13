using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KovakWeb.Data;
using KovakWeb.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace KovakWeb.Controllers
{
    public class KovakAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CLAVE_MAESTRA = "Kovak2026*"; 

        public KovakAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /KovakAdmin?token=Kovak2026*
        public async Task<IActionResult> Index(string token)
        {
            if (token != CLAVE_MAESTRA) return Unauthorized("Acceso no autorizado.");

            var negocios = await _context.Negocios
                .Select(n => new {
                    Negocio = n,
                    CantidadProductos = _context.Productos.Count(p => p.NegocioID == n.ID),
                    Usuario = _context.Usuarios.FirstOrDefault(u => u.NegocioID == n.ID)
                })
                .ToListAsync();

            ViewBag.Token = token;
            ViewBag.Negocios = negocios;
            return View();
        }

        // GET: /KovakAdmin/Registrar
        public IActionResult Registrar(string token)
        {
            if (token != CLAVE_MAESTRA) return Unauthorized();
            ViewBag.Token = token;
            return View();
        }

        // POST: /KovakAdmin/Registrar
        [HttpPost]
        public async Task<IActionResult> Registrar(string token, string nombreNegocio, string whatsapp, string emailAdmin, string passwordAdmin)
        {
            if (token != CLAVE_MAESTRA) return Unauthorized();

            if (string.IsNullOrWhiteSpace(nombreNegocio) || string.IsNullOrWhiteSpace(whatsapp) || 
                string.IsNullOrWhiteSpace(emailAdmin) || string.IsNullOrWhiteSpace(passwordAdmin))
            {
                ViewBag.Error = "Todos los campos son obligatorios.";
                ViewBag.Token = token;
                return View();
            }

            string slug = nombreNegocio.Trim().ToLower().Replace(" ", "-");

            var existeUrl = await _context.Negocios.AnyAsync(n => n.UrlPersonalizada == slug);
            if (existeUrl)
            {
                ViewBag.Error = "Ya existe un comercio con una URL similar basada en ese nombre.";
                ViewBag.Token = token;
                return View();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var nuevoNegocio = new Negocio
                    {
                        Nombre = nombreNegocio.Trim(),
                        WhatsApp = whatsapp.Trim(),
                        UrlPersonalizada = slug,
                        Activo = true
                    };
                    _context.Negocios.Add(nuevoNegocio);
                    await _context.SaveChangesAsync();

                    var nuevoUsuario = new Usuario
                    {
                        Email = emailAdmin.Trim().ToLower(),
                        Password = passwordAdmin.Trim(),
                        NegocioID = nuevoNegocio.ID
                    };
                    _context.Usuarios.Add(nuevoUsuario);

                    var categoriaDefault = new Categoria
                    {
                        Nombre = "General",
                        Activo = true,
                        NegocioID = nuevoNegocio.ID
                    };
                    _context.Categorias.Add(categoriaDefault);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction("Index", new { token = CLAVE_MAESTRA });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ViewBag.Error = $"Error al procesar el alta: {ex.Message}";
                    ViewBag.Token = token;
                    return View();
                }
            }
        }

        // POST: /KovakAdmin/AlternarEstado
        [HttpPost]
        public async Task<IActionResult> AlternarEstado(string token, int id)
        {
            if (token != CLAVE_MAESTRA) return Unauthorized();

            var negocio = await _context.Negocios.FindAsync(id);
            if (negocio != null)
            {
                // Invertimos el estado lógico (Si está activo pasa a falso, si está falso pasa a verdadero)
                negocio.Activo = !negocio.Activo;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { token = CLAVE_MAESTRA });
        }

        // POST: /KovakAdmin/ModificarAcceso
        [HttpPost]
        public async Task<IActionResult> ModificarAcceso(string token, int negocioId, string nuevoEmail, string nuevaPassword)
        {
            if (token != CLAVE_MAESTRA) return Unauthorized();

            if (string.IsNullOrWhiteSpace(nuevoEmail) || string.IsNullOrWhiteSpace(nuevaPassword))
            {
                return RedirectToAction("Index", new { token = CLAVE_MAESTRA });
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.NegocioID == negocioId);
            if (usuario != null)
            {
                usuario.Email = nuevoEmail.Trim().ToLower();
                usuario.Password = nuevaPassword.Trim();
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { token = CLAVE_MAESTRA });
        }

        // POST: /KovakAdmin/Eliminar
        [HttpPost]
        public async Task<IActionResult> Eliminar(string token, int id)
        {
            if (token != CLAVE_MAESTRA) return Unauthorized();

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var productos = _context.Productos.Where(p => p.NegocioID == id);
                    _context.Productos.RemoveRange(productos);

                    var categorias = _context.Categorias.Where(c => c.NegocioID == id);
                    _context.Categorias.RemoveRange(categorias);

                    var usuarios = _context.Usuarios.Where(u => u.NegocioID == id);
                    _context.Usuarios.RemoveRange(usuarios);

                    var negocio = await _context.Negocios.FindAsync(id);
                    if (negocio != null)
                    {
                        _context.Negocios.Remove(negocio);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }

            return RedirectToAction("Index", new { token = CLAVE_MAESTRA });
        }
    }
}