using System.ComponentModel.DataAnnotations;

namespace SistemaMaritimo.Web.Models
{
    public class CambiarEstadoOrdenViewModel
    {
        public int OrdenId { get; set; }

        [Required]
        public string NuevoEstado { get; set; } = string.Empty;
    }
}