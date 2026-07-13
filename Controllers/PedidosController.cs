using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KovakWeb.Data;

namespace KovakWeb.Controllers
{
    public class PedidosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PedidosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Listar todos los pedidos de la base de datos
        public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Pedidos.OrderByDescending(p => p.Fecha).ToListAsync();
            return View(pedidos);
        }

        // 2. Cambiar el estado del pedido (Completado / Cancelado)
        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                pedido.Estado = nuevoEstado;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}