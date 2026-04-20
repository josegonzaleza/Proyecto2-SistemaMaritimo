namespace SistemaMaritimo.API.Models
{
    public class AsignacionTripulacion
    {
        public int Id { get; set; }
        public int PersonalId { get; set; }
        public int BarcoId { get; set; }
        public string PuestoAsignado { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Activa { get; set; }
    }
}