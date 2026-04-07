using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.SIGE.Models
{
    [Table("ProducaoFamilia")]
    public class ProducaoFamilia
    {
        [Key]
        public int IdProducaoFamilia { get; set; }

        [Required]
        [ForeignKey("FamiliaCaixilho")]
        public int IdFamiliaCaixilho { get; set; }

        [Required]
        [ForeignKey("Responsavel")]
        public int IdResponsavel { get; set; }

        [Required]
        public StatusAtividade Status { get; set; } = StatusAtividade.NaoIniciada;

        public DateTime DataInicio { get; set; }

        public DateTime? DataEstimadaConclusao { get; set; }

        public DateTime? DataConclusao { get; set; }

        [StringLength(200)]
        public string? Descricao { get; set; }

        [StringLength(500)]
        public string? Observacoes { get; set; }

        [JsonIgnore]
        public virtual FamiliaCaixilho? FamiliaCaixilho { get; set; }

        [JsonIgnore]
        public virtual Usuario? Responsavel { get; set; }
    }
}
