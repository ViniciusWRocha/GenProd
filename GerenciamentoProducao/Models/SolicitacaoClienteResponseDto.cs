namespace GerenciamentoProducao.Models;

public class SolicitacaoClienteResponseDto
{
    public int IdSolicitacao { get; set; }
    public int IdCaixilho { get; set; }
    public string? NomeCaixilho { get; set; }
    public int IdCliente { get; set; }
    public string? NomeCliente { get; set; }
    public DateTime DataNecessidadeEmObra { get; set; }
    public string? ObservacaoCliente { get; set; }
    public int Prioridade { get; set; }
    public DateTime DataSolicitacao { get; set; }
    public string? NomeObra { get; set; }
    public int IdObra { get; set; }
}