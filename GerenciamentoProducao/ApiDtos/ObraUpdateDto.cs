namespace GerenciamentoProducao.ApiDtos
{
    public class ObraUpdateDto
    {
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
        public string? Observacoes { get; set; }
        public bool Finalizado { get; set; }
        public string? ImagemObraPath { get; set; }
        public int IdUsuario { get; set; }
    }
}
