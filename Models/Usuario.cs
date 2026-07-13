using System.ComponentModel.DataAnnotations;

namespace KovakWeb.Models
{
    public class Usuario
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        // Cada usuario pertenece a un único negocio
        [Required]
        public int NegocioID { get; set; }
    }
}