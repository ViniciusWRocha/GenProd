namespace GerenciamentoProducao.Models;

public class MedicaoResponseDto
{
    public int IdMedicao { get; set; }
    public int IdFamiliaCaixilho { get; set; }
    public int IdResponsavel { get; set; }
    public string? NomeResponsavel { get; set; }
    public int Status { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataEstimadaConclusao { get; set; }
    public DateTime? DataConclusao { get; set; }
    public string? Descricao { get; set; }
    public string? Observacoes { get; set; }

    public string StatusTexto => Status switch
    {
        1 => "Não Iniciada",
        2 => "Em Andamento",
        3 => "Pausada",
        4 => "Concluída",
        _ => "Desconhecido"
    };
}
