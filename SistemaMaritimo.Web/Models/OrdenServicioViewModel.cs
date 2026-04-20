using System.ComponentModel.DataAnnotations;

namespace SistemaMaritimo.Web.Models
{
    public class OrdenServicioViewModel
    {
        public int Id { get; set; }
        public string CodigoOrden { get; set; } = string.Empty;

        [Required]
        public int BarcoId { get; set; }

        [Required]
        public string TipoMantenimiento { get; set; } = string.Empty;

        [Required]
        public string Prioridad { get; set; } = string.Empty;

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaLimite { get; set; }

        public string Estado { get; set; } = "Abierta";
        public string? InformeCierre { get; set; }
        public DateTime? FechaCierreReal { get; set; }
        public string? UsuarioCierre { get; set; }
    }
}