using System.ComponentModel.DataAnnotations;

namespace KovakWeb.Models
{
    public class Pedido
    {
        [Key]
        public int ID { get; set; }
        public int NegocioID { get; set; }
        public string NombreCliente { get; set; } = null!;
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente";
    }
}