namespace SistemaMaritimo.API.Models
{
    public class HistorialTecnicoItem
    {
        public int Id { get; set; }
        public int BarcoId { get; set; }
        public int OrdenServicioId { get; set; }
        public string TipoMantenimiento { get; set; } = string.Empty;
        public string DescripcionTrabajo { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }
}