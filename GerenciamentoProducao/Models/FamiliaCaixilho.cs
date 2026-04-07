using System.ComponentModel.DataAnnotations;

namespace GerenciamentoProducao.Models
{
    public class FamiliaCaixilho
    {
        [Key]
        public int IdFamiliaCaixilho { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Família de Caixilhos deve ter entre 3 e 100 caracteres")]
        public string DescricaoFamilia { get; set; } = string.Empty;

        public int PesoTotal { get; set; }

        public string PesoTotalFormatado => $"{PesoTotal:F2} kg";

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int IdObra { get; set; }

        public string StatusFamilia { get; set; } = "Pendente";

        public int QuantidadeCaixilhos { get; set; }
    }
}
