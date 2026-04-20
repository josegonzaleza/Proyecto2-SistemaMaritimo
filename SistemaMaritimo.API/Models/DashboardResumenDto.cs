namespace SistemaMaritimo.API.Models
{
    public class DashboardResumenDto
    {
        public int BarcosActivos { get; set; }
        public int BarcosArchivados { get; set; }
        public int TravesiasPlaneadas { get; set; }
        public int TravesiasEnCurso { get; set; }
        public int OrdenesAbiertas { get; set; }
        public int OrdenesEnProgreso { get; set; }
        public int LicenciasPorVencer { get; set; }
        public int LicenciasVencidas { get; set; }
    }
}