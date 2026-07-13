using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // REQUERIDO PARA MANEJAR LISTAS RELACIONALES

namespace KovakWeb.Models
{
    public class Producto
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public int Stock { get; set; }

        // Límite estricto: 4 imágenes por producto
        public string? UrlImagen { get; set; }
        public string? UrlImagen2 { get; set; }
        public string? UrlImagen3 { get; set; }
        public string? UrlImagen4 { get; set; }

        public int NegocioID { get; set; }
        public int CategoriaID { get; set; }
        public bool Activo { get; set; } = true;

        // CRÍTICO: El conector que le falta a C# para vincular los talles directamente con el producto
        public List<ProductoTalle> ProductoTalles { get; set; } = new List<ProductoTalle>();
    }
}