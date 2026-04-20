namespace SistemaMaritimo.API.Models
{
    public class DashboardOrdenItemDto
    {
        public int Id { get; set; }
        public string CodigoOrden { get; set; } = string.Empty;
        public string NombreBarco { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaLimite { get; set; }
    }
}