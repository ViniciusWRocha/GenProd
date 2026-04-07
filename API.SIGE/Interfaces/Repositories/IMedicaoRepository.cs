using API.SIGE.Models;

namespace API.SIGE.Interfaces.Repositories
{
    public interface IMedicaoRepository
    {
        Task<List<Medicao>> GetAllAsync();
        Task<Medicao?> GetByIdAsync(int id);
        Task<Medicao?> GetByFamiliaIdAsync(int familiaId);
        Task AddAsync(Medicao medicao);
        Task UpdateAsync(Medicao medicao);
        Task DeleteAsync(int id);
    }
}
