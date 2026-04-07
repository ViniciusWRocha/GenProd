using System.ComponentModel.DataAnnotations;

namespace API.SIGE.DTOs
{
    public class ObraCreateDto
    {
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3)]
        public string Construtora { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100)]
        public string Nro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(200, MinimumLength = 3)]
        public string Logradouro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3)]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(9)]
        public string Cep { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(2)]
        public string Uf { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(14)]
        public string Cnpj { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public DateTime DataTermino { get; set; }

        public float PesoFinal { get; set; }

        [StringLength(500)]
        public string? Observacoes { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int IdUsuario { get; set; }

        public int? IdResponsavelVerificacao { get; set; }
        public int? IdResponsavelMedicao { get; set; }
        public int? IdResponsavelProducao { get; set; }
    }
}
