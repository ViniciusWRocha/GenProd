using System.ComponentModel.DataAnnotations;

namespace GerenciamentoProducao.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(50)]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(50)]
        public string Senha { get; set; } = string.Empty;

        [StringLength(15)]
        public string? Telefone { get; set; }

        public bool Ativo { get; set; } = true;

        public int IdTipoUsuario { get; set; }
        public virtual TipoUsuario? TipoUsuario { get; set; }
    }
}
