namespace SistemaMaritimo.API.Models
{
    public class LicenciaAlertaDto
    {
        public int PersonalId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string NombreLicencia { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}