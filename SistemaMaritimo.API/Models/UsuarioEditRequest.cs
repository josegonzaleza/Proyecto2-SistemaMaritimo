namespace SistemaMaritimo.API.Models
{
    public class UsuarioEditRequest
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public int RolId { get; set; }
        public bool Activo { get; set; }
    }
}