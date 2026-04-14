using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Interfaces
{
    public interface IFamiliaCaixilhoRepository
    {
         Task<List<FamiliaCaixilho>> GetAllAsync();
         Task AddAsync(FamiliaCaixilho familiaCaixilho);
         Task UpdateAsync(FamiliaCaixilho familiaCaixilho);
         Task DeleteAsync(int id);
         Task<FamiliaCaixilho> GetByIdAsync(int id);
         Task<float> CalcularPesoTotalAsync(int familiaId);
         Task AtualizarPesoTotalAsync(int familiaId);
         Task<List<FamiliaCaixilho>> GetByObraIdAsync(int obraId);
        /// <summary>Marca a família como Produzida (API: POST finalizar-producao). Exige status EmProducao.</summary>
        Task FinalizarProducaoAsync(int familiaId);
    }
}
