using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.SIGE.Models
{
    [Table ("Obra")]
    public class Obra
    {
        [Key]
        public int IdObra { get; set; }
        [Required (ErrorMessage ="Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
        public string Nome { get; set; } 

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "A Construtora deve ter entre 3 e 100 caracteres")]
        public string Construtora { get; set; }

        [Required (ErrorMessage ="Campo Obrigatório")]
        [StringLength(100, ErrorMessage = "O Número não permite Nulo")]
        public string Nro { get; set; }


        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Logradouro deve ter entre 3 e 200 caracteres")]
        public string Logradouro { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Bairro deve ter entre 3 e 100 caracteres")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(9,ErrorMessage = "CEP deve ter 9 caracteres")]
        public string Cep { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(2, ErrorMessage = "CEP deve ter 2 caracteres")]
        public string Uf { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(14, ErrorMessage = "CEP deve ter 14  caracteres")]
        public string Cnpj { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime DataTermino { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public float PesoFinal { get; set; }

        public float PesoProduzido { get; set; } = 0;

        //public string StatusObra { get; set; } = "Planejada"; // Planejada, Em Andamento, Concluída, Atrasada

        //public string Prioridade { get; set; } = "Normal"; // Baixa, Normal, Alta, Crítica PODE SER UM ENUM

        //public string Bandeira { get; set; } = "Verde"; // Verde, Amarela, Vermelha, Crítica

        public float PercentualConclusao { get; set; } = 0;

        public DateTime? DataConclusao { get; set; }

        [StringLength(500)]
        public string? Observacoes { get; set; }

        public bool Finalizado { get; set; }

        [StringLength(255)]
        public string? ImagemObraPath { get; set; }

        public StatusObra StatusObra { get; set; } = StatusObra.Cadastrada;

        public float PercentualMedicao { get; set; } = 0;

        public float PercentualProducao { get; set; } = 0;

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        [JsonIgnore]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey("ResponsavelVerificacao")]
        public int? IdResponsavelVerificacao { get; set; }
        [JsonIgnore]
        public virtual Usuario? ResponsavelVerificacao { get; set; }

        [ForeignKey("ResponsavelMedicao")]
        public int? IdResponsavelMedicao { get; set; }
        [JsonIgnore]
        public virtual Usuario? ResponsavelMedicao { get; set; }

        [ForeignKey("ResponsavelProducao")]
        public int? IdResponsavelProducao { get; set; }
        [JsonIgnore]
        public virtual Usuario? ResponsavelProducao { get; set; }
    }
}
