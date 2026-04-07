using System.ComponentModel.DataAnnotations;
using API.SIGE.Models;

namespace API.SIGE.DTOs
{
    public class CaixilhoCreateDto
    {
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3)]
        public string NomeCaixilho { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Largura { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Altura { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public float PesoUnitario { get; set; }

        [StringLength(200)]
        public string? Observacoes { get; set; }

        public string? DescricaoCaixilho { get; set; }

        public StatusProducao StatusProducao { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int ObraId { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int IdFamiliaCaixilho { get; set; }
    }
}
