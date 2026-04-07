namespace GerenciamentoProducao.ViewModel
{
    public class DashboardViewModel
    {
        // Cards superiores
        public int TotalObras { get; set; }
        public int ObrasConcluidas { get; set; }
        public int FamiliasLiberadas { get; set; }
        public int TotalFamilias { get; set; }
        public float PesoLiberadoKg { get; set; }
        public float PesoPendenteKg { get; set; }

        // Situacao de prazo
        public int ObrasEmDia { get; set; }
        public int ObrasEmAlerta { get; set; }
        public int ObrasAtrasadas { get; set; }

        // Obras para tabela de prazo
        public List<ObraDashboardItem> Obras { get; set; } = new();
    }

    public class ObraDashboardItem
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Construtora { get; set; } = string.Empty;
        public string StatusObra { get; set; } = string.Empty;
        public string SituacaoPrazo { get; set; } = string.Empty;
        public float PercentualMedicao { get; set; }
        public float PercentualProducao { get; set; }
        public float PesoFinal { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataTermino { get; set; }
        public int DiasRestantes { get; set; }
    }
}
