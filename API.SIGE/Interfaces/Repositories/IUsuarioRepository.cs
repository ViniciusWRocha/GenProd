using API.SIGE.Models;

namespace API.SIGE.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> GetAllAsync();
        Task<Usuario?> GetById(int id);
        Task<List<Usuario>> GetAllAtivosAsync();
        Task<List<Usuario>> GetAllInativosAsync();
        Task AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task Delete(int id);
        Task InativarAsync(int id);
        Task ReativarAsync(int id);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<List<Usuario>> GetByCargoAsync(TipoCargo tipoCargo);
    }
}
