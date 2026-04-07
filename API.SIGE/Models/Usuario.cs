using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.SIGE.Models
{
    [Table ("Usuario")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength (50)]
        public string NomeUsuario { get; set; }
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(200)]
        public string Senha { get; set; }
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(14)]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public bool Ativo { get; set; } = true;

        [ForeignKey("TipoUsuario")]
        public int IdTipoUsuario { get; set; }
        [JsonIgnore]
        public virtual TipoUsuario? TipoUsuario { get; set; }

        public virtual ICollection<UsuarioCargo>? UsuarioCargos { get; set; }
    }
}
