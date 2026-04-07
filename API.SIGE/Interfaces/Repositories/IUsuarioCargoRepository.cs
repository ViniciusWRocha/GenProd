using API.SIGE.Models;

namespace API.SIGE.Interfaces.Repositories
{
    public interface IUsuarioCargoRepository
    {
        Task<List<UsuarioCargo>> GetByUsuarioIdAsync(int idUsuario);
        Task AddAsync(UsuarioCargo usuarioCargo);
        Task DeleteAsync(int idUsuario, int idCargo);
        Task<bool> ExistsAsync(int idUsuario, int idCargo);
    }
}
