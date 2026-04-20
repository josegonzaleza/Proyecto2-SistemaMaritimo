using System.ComponentModel.DataAnnotations;

namespace SistemaMaritimo.Web.Models
{
    public class AsignarTecnicoViewModel
    {
        public int OrdenId { get; set; }

        [Required]
        public int PersonalId { get; set; }
    }
}