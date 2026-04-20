using System.ComponentModel.DataAnnotations;

namespace SistemaMaritimo.Web.Models
{
    public class CambiarEstadoTravesiaViewModel
    {
        public int Id { get; set; }

        [Required]
        public string NuevoEstado { get; set; } = string.Empty;
    }
}