namespace GerenciamentoProducao.ApiDtos
{
    public class ObraDashboardDto
    {
        public int IdObra { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string StatusObra { get; set; } = string.Empty;
        public float PercentualMedicao { get; set; }
        public float PercentualProducao { get; set; }
    }
}
