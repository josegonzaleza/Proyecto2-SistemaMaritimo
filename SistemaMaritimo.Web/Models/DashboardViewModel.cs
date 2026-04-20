namespace SistemaMaritimo.Web.Models
{
    public class DashboardViewModel
    {
        public DashboardResumenViewModel Resumen { get; set; } = new();
        public List<LicenciaAlertaViewModel> LicenciasAlertas { get; set; } = new();
        public List<DashboardOrdenItemViewModel> OrdenesPendientes { get; set; } = new();
        public List<DashboardTravesiaItemViewModel> TravesiasActivas { get; set; } = new();
    }
}