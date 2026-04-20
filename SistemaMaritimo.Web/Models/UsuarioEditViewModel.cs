using System.ComponentModel.DataAnnotations;

namespace SistemaMaritimo.Web.Models
{
    public class UsuarioEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        public int RolId { get; set; }

        public bool Activo { get; set; }
    }
}