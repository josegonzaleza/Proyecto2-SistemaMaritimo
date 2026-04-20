namespace SistemaMaritimo.API.Models
{
    public class UsuarioCreateRequest
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
        public int RolId { get; set; }
    }
}