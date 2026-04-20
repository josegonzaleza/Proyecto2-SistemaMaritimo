namespace SistemaMaritimo.Web.Models
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public string Roles { get; set; } = string.Empty;
    }
}