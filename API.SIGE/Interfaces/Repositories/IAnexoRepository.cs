using API.SIGE.Models;

namespace API.SIGE.Interfaces.Repositories
{
    public interface IAnexoRepository
    {
        Task<Anexo?> GetByIdAsync(int id);
        Task<List<Anexo>> GetByMedicaoIdAsync(int medicaoId);
        Task<List<Anexo>> GetByProducaoFamiliaIdAsync(int producaoFamiliaId);
        Task AddAsync(Anexo anexo);
        Task DeleteAsync(int id);
    }
}
