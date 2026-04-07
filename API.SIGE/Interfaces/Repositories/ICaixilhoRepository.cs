using API.SIGE.Models;

namespace API.SIGE.Interfaces.Repositories
{
    public interface ICaixilhoRepository
    {
        Task<List<Caixilho>> GetAllAsync();
        Task<Caixilho?> GetById(int id);
        Task<Caixilho?> GetByFamilia(Caixilho caixilho);
        Task AddAsync(Caixilho caixilho);
        Task UpdateAsync(Caixilho caixilho);
        Task DeleteAsync(int id);
    }
}
