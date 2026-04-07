using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.SIGE.Models
{
    [Table("FamiliaCaixilho")]
    public class FamiliaCaixilho
    {
        [Key]
        public int IdFamiliaCaixilho { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Familia de Caixilhos deve ter entre 3 e 100 caracteres")]
        public string DescricaoFamilia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int PesoTotal { get; set; }

        [NotMapped]
        public string PesoTotalFormatado => $"{PesoTotal:F2} kg";

        [Required]
        [ForeignKey("Obra")]
        public int IdObra { get; set; }

        [JsonIgnore]
        public virtual Obra? Obra { get; set; }

        public StatusFamilia StatusFamilia { get; set; } = StatusFamilia.Pendente;

        public int QuantidadeCaixilhos { get; set; }
    }
}
