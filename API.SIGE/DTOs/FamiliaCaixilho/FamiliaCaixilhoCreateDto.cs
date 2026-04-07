using System.ComponentModel.DataAnnotations;

namespace API.SIGE.DTOs
{
    public class FamiliaCaixilhoCreateDto
    {
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3)]
        public string DescricaoFamilia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int IdObra { get; set; }
    }
}
