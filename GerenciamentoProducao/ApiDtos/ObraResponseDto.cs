namespace GerenciamentoProducao.ApiDtos
{
    public class ObraResponseDto
    {
        public int IdObra { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Construtora { get; set; } = string.Empty;
        public string Nro { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime DataTermino { get; set; }
        public float PesoFinal { get; set; }
        public float PesoProduzido { get; set; }
        public float PercentualConclusao { get; set; }
        public DateTime? DataConclusao { get; set; }
        public string? Observacoes { get; set; }
        public bool Finalizado { get; set; }
        public string? ImagemObraPath { get; set; }
        public int IdUsuario { get; set; }
        public int? IdCliente { get; set; }
        public string? NomeUsuario { get; set; }
        public int StatusObra { get; set; }
        public float PercentualMedicao { get; set; }
        public float PercentualProducao { get; set; }
    }
}
