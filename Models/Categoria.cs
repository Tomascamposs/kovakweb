using System.ComponentModel.DataAnnotations;

namespace KovakWeb.Models
{
    public class Categoria
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        public string Nombre { get; set; } = string.Empty;
        
        public int NegocioID { get; set; }
        
        public bool Activo { get; set; } = true;
    }
}