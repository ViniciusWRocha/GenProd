using System.ComponentModel.DataAnnotations;

namespace GerenciamentoProducao.Models
{
    public class Caixilho
    {
        [Key]
        public int IdCaixilho { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome do Caixilho deve ter entre 3 e 100 caracteres")]
        public string NomeCaixilho { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Largura { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Altura { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public float PesoUnitario { get; set; }

        public bool Liberado { get; set; } = false;

        public DateTime? DataLiberacao { get; set; }

        [StringLength(200)]
        public string? Observacoes { get; set; }

        [StringLength(100)]
        public string? DescricaoCaixilho { get; set; }

        public string StatusProducao { get; set; } = "Pendente";

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int ObraId { get; set; }
        public virtual Obra? Obra { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int IdFamiliaCaixilho { get; set; }
        public virtual FamiliaCaixilho? FamiliaCaixilho { get; set; }
    }
}
