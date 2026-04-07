using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.SIGE.Models
{
    [Table("Notificacao")]
    public class Notificacao
    {
        [Key]
        public int IdNotificacao { get; set; }

        [Required]
        [ForeignKey("UsuarioDestino")]
        public int IdUsuarioDestino { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Mensagem { get; set; } = string.Empty;

        public bool Lida { get; set; } = false;

        public DateTime DataCriacao { get; set; }

        [Required]
        public TipoNotificacao TipoNotificacao { get; set; }

        [ForeignKey("Obra")]
        public int? IdObra { get; set; }

        [JsonIgnore]
        public virtual Usuario? UsuarioDestino { get; set; }

        [JsonIgnore]
        public virtual Obra? Obra { get; set; }
    }
}
