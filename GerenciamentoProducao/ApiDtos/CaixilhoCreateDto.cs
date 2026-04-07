namespace GerenciamentoProducao.ApiDtos
{
    public class CaixilhoCreateDto
    {
        public string NomeCaixilho { get; set; } = string.Empty;
        public int Largura { get; set; }
        public int Altura { get; set; }
        public int Quantidade { get; set; }
        public float PesoUnitario { get; set; }
        public string? Observacoes { get; set; }
        public string? DescricaoCaixilho { get; set; }
        public int StatusProducao { get; set; }
        public int ObraId { get; set; }
        public int IdFamiliaCaixilho { get; set; }
    }
}
