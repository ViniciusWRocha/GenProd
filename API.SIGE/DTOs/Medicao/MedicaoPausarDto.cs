using System.ComponentModel.DataAnnotations;

namespace API.SIGE.DTOs
{
    public class MedicaoPausarDto
    {
        [StringLength(500)]
        public string? Observacoes { get; set; }
    }
}
