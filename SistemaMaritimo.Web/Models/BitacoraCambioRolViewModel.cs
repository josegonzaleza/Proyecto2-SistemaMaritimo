namespace SistemaMaritimo.Web.Models
{
    public class BitacoraCambioRolViewModel
    {
        public int Id { get; set; }
        public string UsuarioAfectado { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string RealizadoPor { get; set; } = string.Empty;
    }
}