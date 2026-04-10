using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Interfaces;

public interface IFamiliaMedicaoFotoStore
{
    Task<FamiliaMedicaoFotoState?> GetAsync(int idFamiliaCaixilho, CancellationToken cancellationToken = default);

    Task SaveAsync(FamiliaMedicaoFotoState state, CancellationToken cancellationToken = default);

    Task DeleteAsync(int idFamiliaCaixilho, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> ListFamiliasComFotoPendenteAsync(CancellationToken cancellationToken = default);
}
