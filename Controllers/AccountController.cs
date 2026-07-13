using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using KovakWeb.Data;
using System;
using System.Linq;

namespace KovakWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Si ya estaba logueado, lo mandamos directo a sus productos
            if (HttpContext.Session.GetInt32("NegocioID") != null)
            {
                return RedirectToAction("Index", "Productos");
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // CONTROL RIGUROSO: Validar que no lleguen vacíos antes de operar con los strings
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Por favor, complete todos los campos.";
                return View();
            }

            // Buscamos el usuario ignorando diferencias de mayúsculas/minúsculas en el email
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email.ToLower() == email.Trim().ToLower() && u.Password == password.Trim());

            if (usuario != null)
            {
                // CONTROL DE SEGURIDAD INTERNO: Evitar quiebre si el negocioID está en 0 o vacío
                if (usuario.NegocioID == 0)
                {
                    ViewBag.Error = "El usuario no tiene un negocio asignado en el sistema.";
                    return View();
                }

                // ¡Éxito! Guardamos las credenciales seguras en la memoria de la sesión
                HttpContext.Session.SetInt32("NegocioID", usuario.NegocioID);
                HttpContext.Session.SetString("UsuarioEmail", usuario.Email);

                return RedirectToAction("Index", "Productos");
            }

            // Si falla, devolvemos un mensaje de error a la pantalla
            ViewBag.Error = "El email o la contraseña son incorrectos.";
            return View();
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Borramos la memoria al salir
            return RedirectToAction("Login");
        }
    }
}