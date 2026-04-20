using System.ComponentModel.DataAnnotations;

namespace SistemaMaritimo.Web.Models
{
    public class CerrarOrdenViewModel
    {
        public int OrdenId { get; set; }

        [Required]
        public string InformeCierre { get; set; } = string.Empty;
    }
}