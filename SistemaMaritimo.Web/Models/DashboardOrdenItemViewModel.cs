namespace SistemaMaritimo.Web.Models
{
    public class DashboardOrdenItemViewModel
    {
        public int Id { get; set; }
        public string CodigoOrden { get; set; } = string.Empty;
        public string NombreBarco { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaLimite { get; set; }
    }
}