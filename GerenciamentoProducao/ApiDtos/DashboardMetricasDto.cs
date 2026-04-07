namespace GerenciamentoProducao.ApiDtos
{
    public class DashboardMetricasDto
    {
        public int TotalObras { get; set; }
        public int TotalCaixilhos { get; set; }
        public int TotalUsuarios { get; set; }
        public int ObrasEmAndamento { get; set; }
        public List<ObraDashboardDto> ObrasProgresso { get; set; } = new();
    }
}
