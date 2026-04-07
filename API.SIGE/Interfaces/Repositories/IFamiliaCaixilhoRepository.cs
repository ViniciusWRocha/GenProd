using API.SIGE.Models;

namespace API.SIGE.Interfaces.Repositories
{
    public interface IFamiliaCaixilhoRepository
    {
        Task<List<FamiliaCaixilho>> GetAllAsync();
        Task AddAsync(FamiliaCaixilho familiaCaixilho);
        Task UpdateAsync(FamiliaCaixilho familiaCaixilho);
        Task DeleteAsync(int id);
        Task<FamiliaCaixilho?> GetByIdAsync(int id);
        Task<List<FamiliaCaixilho>> GetByObraIdAsync(int obraId);
    }
}
