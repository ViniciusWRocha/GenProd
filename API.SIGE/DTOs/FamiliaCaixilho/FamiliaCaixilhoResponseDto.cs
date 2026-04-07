namespace API.SIGE.DTOs
{
    public class FamiliaCaixilhoResponseDto
    {
        public int IdFamiliaCaixilho { get; set; }
        public string DescricaoFamilia { get; set; } = string.Empty;
        public int PesoTotal { get; set; }
        public string PesoTotalFormatado => $"{PesoTotal:F2} kg";
        public int IdObra { get; set; }
        public string? NomeObra { get; set; }
        public string StatusFamilia { get; set; } = string.Empty;
        public int QuantidadeCaixilhos { get; set; }
    }
}
