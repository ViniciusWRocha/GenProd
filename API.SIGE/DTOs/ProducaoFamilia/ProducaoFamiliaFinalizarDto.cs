using System.ComponentModel.DataAnnotations;

namespace API.SIGE.DTOs
{
    public class ProducaoFamiliaFinalizarDto
    {
        [StringLength(500)]
        public string? Observacoes { get; set; }
    }
}
