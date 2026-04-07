using API.SIGE.Models;

namespace API.SIGE.Interfaces.Repositories
{
    public interface ITipoUsuarioRepository
    {
        Task<List<TipoUsuario>> GetAllAsync();
        Task<TipoUsuario?> GetById(int id);
        Task AddAsync(TipoUsuario tipoUsuario);
        Task UpdateAsync(TipoUsuario tipoUsuario);
        Task DeleteAsync(int id);
    }
}
