using System.ComponentModel.DataAnnotations;

namespace KovakWeb.Models
{
    public class Negocio
    {
        [Key]
        public int ID { get; set; }
        public string Nombre { get; set; } = null!;
        public string WhatsApp { get; set; } = null!;
        public string UrlPersonalizada { get; set; } = null!;
        public bool Activo { get; set; } = true;
    }
}