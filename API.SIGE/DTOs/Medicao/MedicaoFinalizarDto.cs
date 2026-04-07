using System.ComponentModel.DataAnnotations;

namespace API.SIGE.DTOs
{
    public class MedicaoFinalizarDto
    {
        [StringLength(500)]
        public string? Observacoes { get; set; }
    }
}
