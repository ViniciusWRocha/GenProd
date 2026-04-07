using API.SIGE.Models;

namespace API.SIGE.Interfaces.Repositories
{
    public interface ICargoRepository
    {
        Task<List<Cargo>> GetAllAsync();
        Task<Cargo?> GetByIdAsync(int id);
        Task<Cargo?> GetByTipoAsync(TipoCargo tipo);
        Task AddAsync(Cargo cargo);
        Task UpdateAsync(Cargo cargo);
        Task DeleteAsync(int id);
    }
}
