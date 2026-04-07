using Microsoft.AspNetCore.Mvc.Rendering;

namespace GerenciamentoProducaoo.ViewModel
{
    public class CaixilhoViewModel
    {
        public int IdCaixilho { get; set; }
        public string NomeCaixilho { get; set; }
        public int Largura { get; set; }
        public int Altura { get; set; }
        public int Quantidade { get; set; }
        public float PesoUnitario { get; set; }

        public int ObraId { get; set; }
        public int FamiliaCaixilhoId { get; set; }
        public int IdFamiliaCaixilho { get; set; }

        // Descricao do tipo de caixilho (substituiu TipoCaixilho)
        public string? DescricaoCaixilho { get; set; }

        public bool Liberado { get; set; } = false;
        public DateTime? DataLiberacao { get; set; }
        public string StatusProducao { get; set; } = "Pendente";
        public string? Observacoes { get; set; }

        public IEnumerable<SelectListItem> Obra { get; set; }
        public IEnumerable<SelectListItem> FamiliaCaixilho { get; set; }
    }
}
