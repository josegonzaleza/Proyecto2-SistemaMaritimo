namespace SistemaMaritimo.API.Models
{
    public class CerrarOrdenRequest
    {
        public string InformeCierre { get; set; } = string.Empty;
        public string UsuarioCierre { get; set; } = string.Empty;
    }
}