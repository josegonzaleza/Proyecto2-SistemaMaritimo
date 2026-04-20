namespace SistemaMaritimo.API.Models
{
    public class CambiarEstadoTravesiaRequest
    {
        public string NuevoEstado { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
    }
}