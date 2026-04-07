using System.ComponentModel.DataAnnotations;

namespace GerenciamentoProducao.Models
{
    public class TipoUsuario
    {
        [Key]
        public int IdTipoUsuario { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100)]
        public string NomeTipoUsuario { get; set; } = string.Empty;

        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
