namespace SistemaMaritimo.API.Models
{
    public class DashboardDto
    {
        public DashboardResumenDto Resumen { get; set; } = new();
        public List<LicenciaAlertaDto> LicenciasAlertas { get; set; } = new();
        public List<DashboardOrdenItemDto> OrdenesPendientes { get; set; } = new();
        public List<DashboardTravesiaItemDto> TravesiasActivas { get; set; } = new();
    }
}