using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KovakWeb.Data;

namespace KovakWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Calculamos las métricas en tiempo real
            ViewBag.TotalProductos = await _context.Productos.CountAsync(p => p.Activo);
            ViewBag.StockBajo = await _context.Productos.CountAsync(p => p.Stock <= 2 && p.Activo);
            ViewBag.PedidosPendientes = await _context.Pedidos.CountAsync(p => p.Estado == "Pendiente");

            // Traemos solo los últimos 5 pedidos para la vista rápida
            var ultimosPedidos = await _context.Pedidos
                .OrderByDescending(p => p.Fecha)
                .Take(5)
                .ToListAsync();

            return View(ultimosPedidos);
        }
    }
}