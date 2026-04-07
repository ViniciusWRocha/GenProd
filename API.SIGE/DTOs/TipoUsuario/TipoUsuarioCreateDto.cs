using System.ComponentModel.DataAnnotations;

namespace API.SIGE.DTOs
{
    public class TipoUsuarioCreateDto
    {
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100)]
        public string NomeTipoUsuario { get; set; } = string.Empty;
    }
}
