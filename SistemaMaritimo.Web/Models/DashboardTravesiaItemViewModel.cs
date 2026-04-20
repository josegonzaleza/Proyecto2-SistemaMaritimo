namespace SistemaMaritimo.Web.Models
{
    public class DashboardTravesiaItemViewModel
    {
        public int Id { get; set; }
        public string NombreBarco { get; set; } = string.Empty;
        public string PuertoOrigen { get; set; } = string.Empty;
        public string PuertoDestino { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaPrevistaSalida { get; set; }
    }
}