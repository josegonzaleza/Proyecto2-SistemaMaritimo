namespace SistemaMaritimo.API.Models
{
    public class Personal
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string IdentificacionUnica { get; set; } = string.Empty;
        public string RolPrimario { get; set; } = string.Empty;
        public DateTime FechaContratacion { get; set; }
        public bool Activo { get; set; }
    }
}