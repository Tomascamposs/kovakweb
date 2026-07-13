using System.ComponentModel.DataAnnotations;

namespace KovakWeb.Models
{
    public class ProductoTalle
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int ProductoID { get; set; }

        // Puede guardar letras "S", "M" o números de zapatillas "40", "41"
        [Required]
        public string Talle { get; set; } = string.Empty; 

        // El stock específico para este talle
        [Required]
        public int Cantidad { get; set; }
    }
}