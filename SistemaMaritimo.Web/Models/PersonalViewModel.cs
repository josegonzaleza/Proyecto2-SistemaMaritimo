using System.ComponentModel.DataAnnotations;

namespace SistemaMaritimo.Web.Models
{
    public class PersonalViewModel
    {
        public int Id { get; set; }

        [Required]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        public string IdentificacionUnica { get; set; } = string.Empty;

        [Required]
        public string RolPrimario { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaContratacion { get; set; }

        public bool Activo { get; set; }
    }
}