using API.SIGE.Models;

namespace API.SIGE.DTOs
{
    public class CaixilhoResponseDto
    {
        public int IdCaixilho { get; set; }
        public string NomeCaixilho { get; set; } = string.Empty;
        public int Largura { get; set; }
        public int Altura { get; set; }
        public int Quantidade { get; set; }
        public float PesoUnitario { get; set; }
        public bool Liberado { get; set; }
        public DateTime? DataLiberacao { get; set; }
        public string? Observacoes { get; set; }
        public string? DescricaoCaixilho { get; set; }
        public StatusProducao StatusProducao { get; set; }
        public int ObraId { get; set; }
        public string? NomeObra { get; set; }
        public int IdFamiliaCaixilho { get; set; }
        public string? DescricaoFamilia { get; set; }
    }
}
