namespace GerenciamentoProducao.Models;

/// <summary>
/// Estado local da medição com foto (família de caixilhos), até aprovação ou rejeição na web.
/// </summary>
public class FamiliaMedicaoFotoState
{
    public int IdFamiliaCaixilho { get; set; }

    /// <summary>Data URL (ex.: data:image/jpeg;base64,...) ou só base64; usado para exibir na web.</summary>
    public string FotoBase64 { get; set; } = string.Empty;

    public DateTime EnviadoEm { get; set; }

    public string? EnviadoPor { get; set; }
}
