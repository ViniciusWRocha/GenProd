using System.ComponentModel.DataAnnotations;

namespace API.SIGE.DTOs
{
    public class ProducaoFamiliaPausarDto
    {
        [StringLength(500)]
        public string? Observacoes { get; set; }
    }
}
