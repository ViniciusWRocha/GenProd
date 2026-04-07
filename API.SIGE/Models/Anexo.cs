using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.SIGE.Models
{
    [Table("Anexo")]
    public class Anexo
    {
        [Key]
        public int IdAnexo { get; set; }

        [Required]
        [StringLength(255)]
        public string NomeArquivo { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string CaminhoArquivo { get; set; } = string.Empty;

        [StringLength(100)]
        public string? TipoArquivo { get; set; }

        public long TamanhoBytes { get; set; }

        public byte[]? Dados { get; set; }

        public DateTime DataUpload { get; set; }

        [Required]
        public TipoAnexo TipoAnexo { get; set; }

        [ForeignKey("Medicao")]
        public int? IdMedicao { get; set; }

        [ForeignKey("ProducaoFamilia")]
        public int? IdProducaoFamilia { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [JsonIgnore]
        public virtual Medicao? Medicao { get; set; }

        [JsonIgnore]
        public virtual ProducaoFamilia? ProducaoFamilia { get; set; }

        [JsonIgnore]
        public virtual Usuario? Usuario { get; set; }
    }
}
