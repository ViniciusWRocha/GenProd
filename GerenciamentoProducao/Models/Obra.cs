using System.ComponentModel.DataAnnotations;

namespace GerenciamentoProducao.Models
{
    public class Obra
    {
        [Key]
        public int IdObra { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "A Construtora deve ter entre 3 e 100 caracteres")]
        public string Construtora { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, ErrorMessage = "O Número não permite Nulo")]
        public string Nro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Logradouro deve ter entre 3 e 200 caracteres")]
        public string Logradouro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Bairro deve ter entre 3 e 100 caracteres")]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(9, ErrorMessage = "CEP deve ter 9 caracteres")]
        public string Cep { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(2, ErrorMessage = "UF deve ter 2 caracteres")]
        public string Uf { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(14, ErrorMessage = "CNPJ deve ter 14 caracteres")]
        public string Cnpj { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime DataTermino { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public float PesoFinal { get; set; }

        public float PesoProduzido { get; set; } = 0;

        public string StatusObra { get; set; } = "Cadastrada";

        public float PercentualConclusao { get; set; } = 0;
        public float PercentualMedicao { get; set; } = 0;
        public float PercentualProducao { get; set; } = 0;

        public DateTime? DataConclusao { get; set; }

        [StringLength(500)]
        public string? Observacoes { get; set; }

        public bool Finalizado { get; set; }

        [StringLength(255)]
        public string? ImagemObraPath { get; set; }

        public int IdUsuario { get; set; }
        public virtual Usuario? Usuario { get; set; }

        // Responsáveis
        public int? IdResponsavelVerificacao { get; set; }
        public string? NomeResponsavelVerificacao { get; set; }
        public int? IdResponsavelMedicao { get; set; }
        public string? NomeResponsavelMedicao { get; set; }
        public int? IdResponsavelProducao { get; set; }
        public string? NomeResponsavelProducao { get; set; }
    }
}
