using API.SIGE.Models;

namespace API.SIGE.Interfaces.Repositories
{
    public interface INotificacaoRepository
    {
        Task<List<Notificacao>> GetByUsuarioIdAsync(int idUsuario);
        Task<List<Notificacao>> GetNaoLidasAsync(int idUsuario);
        Task<Notificacao?> GetByIdAsync(int id);
        Task AddAsync(Notificacao notificacao);
        Task UpdateAsync(Notificacao notificacao);
    }
}
