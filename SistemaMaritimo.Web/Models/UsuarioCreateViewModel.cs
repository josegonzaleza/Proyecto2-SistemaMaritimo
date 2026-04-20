using System.ComponentModel.DataAnnotations;

namespace SistemaMaritimo.Web.Models
{
    public class UsuarioCreateViewModel
    {
        [Required]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Clave { get; set; } = string.Empty;

        [Required]
        public int RolId { get; set; }
    }
}