using System.ComponentModel.DataAnnotations;

namespace API.SIGE.DTOs
{
    public class AnexoUploadDto
    {
        [Required]
        public IFormFile Arquivo { get; set; } = null!;

        [Required]
        public int TipoAnexo { get; set; }

        public int? IdMedicao { get; set; }

        public int? IdProducaoFamilia { get; set; }

        [Required]
        public int IdUsuario { get; set; }
    }
}
