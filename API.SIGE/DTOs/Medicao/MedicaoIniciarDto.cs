using System.ComponentModel.DataAnnotations;

namespace API.SIGE.DTOs
{
    public class MedicaoIniciarDto
    {
        [Required]
        public int IdResponsavel { get; set; }

        public DateTime? DataEstimadaConclusao { get; set; }

        [StringLength(200)]
        public string? Descricao { get; set; }

        [StringLength(500)]
        public string? Observacoes { get; set; }
    }
}
