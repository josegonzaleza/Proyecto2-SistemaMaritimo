using System.ComponentModel.DataAnnotations;

namespace SistemaMaritimo.Web.Models
{
    public class TravesiaViewModel
    {
        public int Id { get; set; }

        [Required]
        public int BarcoId { get; set; }

        [Required]
        public string PuertoOrigen { get; set; } = string.Empty;

        [Required]
        public string PuertoDestino { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime FechaPrevistaSalida { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime FechaPrevistaLlegada { get; set; }

        public string Estado { get; set; } = "Planeada";
        public DateTime? FechaCierreReal { get; set; }
        public string? UsuarioCierre { get; set; }
    }
}