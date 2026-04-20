using System.ComponentModel.DataAnnotations;

namespace SistemaMaritimo.Web.Models
{
    public class AsignacionTripulacionViewModel
    {
        public int Id { get; set; }
        public int PersonalId { get; set; }

        [Required]
        public int BarcoId { get; set; }

        [Required]
        public string PuestoAsignado { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaFin { get; set; }

        public bool Activa { get; set; }
    }
}