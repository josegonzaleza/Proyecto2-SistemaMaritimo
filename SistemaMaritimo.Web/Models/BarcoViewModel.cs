using System.ComponentModel.DataAnnotations;

namespace SistemaMaritimo.Web.Models
{
    public class BarcoViewModel
    {
        public int Id { get; set; }

        [Required]
        public string NombreEmbarcacion { get; set; } = string.Empty;

        [Required]
        public string MatriculaUnica { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "La capacidad debe ser mayor que cero.")]
        public decimal CapacidadCarga { get; set; }

        [Required]
        public string PuertoBase { get; set; } = string.Empty;

        [Required]
        public string ModeloMotor { get; set; } = string.Empty;

        [Required]
        public string PotenciaMotor { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        public int HorasUsoMotor { get; set; }

        public bool Archivado { get; set; }
    }
}