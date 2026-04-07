using System.ComponentModel.DataAnnotations;
using API.SIGE.Models;

namespace API.SIGE.DTOs
{
    public class FamiliaCaixilhoUpdateDto
    {
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3)]
        public string DescricaoFamilia { get; set; } = string.Empty;

        public StatusFamilia? StatusFamilia { get; set; } = null;
    }
}
