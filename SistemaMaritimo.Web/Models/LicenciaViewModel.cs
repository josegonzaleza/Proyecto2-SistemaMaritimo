using System.ComponentModel.DataAnnotations;

namespace SistemaMaritimo.Web.Models
{
    public class LicenciaViewModel
    {
        public int Id { get; set; }
        public int PersonalId { get; set; }

        [Required]
        public string NombreLicencia { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        public string EstadoVisual
        {
            get
            {
                var dias = (FechaVencimiento.Date - DateTime.Today).TotalDays;

                if (dias < 0) return "Vencida";
                if (dias <= 60) return "Por vencer";
                return "Vigente";
            }
        }
    }
}