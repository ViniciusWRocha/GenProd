using System.ComponentModel.DataAnnotations;

namespace API.SIGE.DTOs
{
    public class CargoCreateDto
    {
        [Required]
        public int TipoCargo { get; set; }

        [Required]
        [StringLength(100)]
        public string DescricaoCargo { get; set; } = string.Empty;
    }
}
