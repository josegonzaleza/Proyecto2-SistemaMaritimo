namespace SistemaMaritimo.API.Models
{
    public class TravesiaListItem
    {
        public int Id { get; set; }
        public int BarcoId { get; set; }
        public string NombreBarco { get; set; } = string.Empty;
        public string PuertoOrigen { get; set; } = string.Empty;
        public string PuertoDestino { get; set; } = string.Empty;
        public DateTime FechaPrevistaSalida { get; set; }
        public DateTime FechaPrevistaLlegada { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaCierreReal { get; set; }
        public string? UsuarioCierre { get; set; }
    }
}