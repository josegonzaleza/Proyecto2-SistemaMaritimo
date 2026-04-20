using System.ComponentModel.DataAnnotations;

namespace SistemaMaritimo.Web.Models
{
    public class CambiarPasswordViewModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NuevaClave { get; set; } = string.Empty;
    }
}