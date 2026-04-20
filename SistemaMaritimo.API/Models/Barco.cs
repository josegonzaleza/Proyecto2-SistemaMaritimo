namespace SistemaMaritimo.API.Models
{
    public class Barco
    {
        public int Id { get; set; }
        public string NombreEmbarcacion { get; set; } = string.Empty;
        public string MatriculaUnica { get; set; } = string.Empty;
        public decimal CapacidadCarga { get; set; }
        public string PuertoBase { get; set; } = string.Empty;
        public string ModeloMotor { get; set; } = string.Empty;
        public string PotenciaMotor { get; set; } = string.Empty;
        public int HorasUsoMotor { get; set; }
        public bool Archivado { get; set; }
    }
}