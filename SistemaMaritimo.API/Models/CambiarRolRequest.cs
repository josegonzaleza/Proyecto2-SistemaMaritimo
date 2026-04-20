namespace SistemaMaritimo.API.Models
{
    public class CambiarRolRequest
    {
        public int RolId { get; set; }
        public string RealizadoPor { get; set; } = string.Empty;
    }
}