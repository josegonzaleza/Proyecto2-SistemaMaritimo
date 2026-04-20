namespace SistemaMaritimo.Web.Models
{
    public class OrdenServicioListItemViewModel
    {
        public int Id { get; set; }
        public string CodigoOrden { get; set; } = string.Empty;
        public int BarcoId { get; set; }
        public string NombreBarco { get; set; } = string.Empty;
        public string TipoMantenimiento { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaLimite { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? TecnicoAsignado { get; set; }
        public DateTime? FechaCierreReal { get; set; }
        public string? UsuarioCierre { get; set; }
    }
}