namespace SistemaMaritimo.API.Models
{
    public class LicenciaMaritima
    {
        public int Id { get; set; }
        public int PersonalId { get; set; }
        public string NombreLicencia { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
    }
}