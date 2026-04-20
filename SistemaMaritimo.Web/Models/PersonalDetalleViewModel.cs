namespace SistemaMaritimo.Web.Models
{
    public class PersonalDetalleViewModel
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string IdentificacionUnica { get; set; } = string.Empty;
        public string RolPrimario { get; set; } = string.Empty;
        public DateTime FechaContratacion { get; set; }
        public bool Activo { get; set; }

        public string? BarcoActual { get; set; }
        public string? PuestoActual { get; set; }

        public List<LicenciaViewModel> Licencias { get; set; } = new();
        public List<HistorialAsignacionViewModel> HistorialAsignaciones { get; set; } = new();
    }

    public class HistorialAsignacionViewModel
    {
        public int Id { get; set; }
        public string NombreBarco { get; set; } = string.Empty;
        public string PuestoAsignado { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Activa { get; set; }
    }
}