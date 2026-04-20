using System.ComponentModel.DataAnnotations;

namespace SistemaMaritimo.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Clave { get; set; } = string.Empty;
    }
}