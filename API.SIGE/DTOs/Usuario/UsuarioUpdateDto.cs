using System.ComponentModel.DataAnnotations;

namespace API.SIGE.DTOs
{
    public class UsuarioUpdateDto
    {
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(50)]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(50, MinimumLength = 6)]
        public string? Senha { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(14)]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int IdTipoUsuario { get; set; }
    }
}
