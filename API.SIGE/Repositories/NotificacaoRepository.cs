using API.SIGE.Data;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Repositories
{
    public class NotificacaoRepository : INotificacaoRepository
    {
        private readonly AppDbData _context;

        public NotificacaoRepository(AppDbData context)
        {
            _context = context;
        }

        public async Task<List<Notificacao>> GetByUsuarioIdAsync(int idUsuario)
        {
            return await _context.Notificacoes
                .Include(n => n.Obra)
                .Where(n => n.IdUsuarioDestino == idUsuario)
                .OrderByDescending(n => n.DataCriacao)
                .ToListAsync();
        }

        public async Task<List<Notificacao>> GetNaoLidasAsync(int idUsuario)
        {
            return await _context.Notificacoes
                .Include(n => n.Obra)
                .Where(n => n.IdUsuarioDestino == idUsuario && !n.Lida)
                .OrderByDescending(n => n.DataCriacao)
                .ToListAsync();
        }

        public async Task<Notificacao?> GetByIdAsync(int id)
        {
            return await _context.Notificacoes
                .Include(n => n.Obra)
                .FirstOrDefaultAsync(n => n.IdNotificacao == id);
        }

        public async Task AddAsync(Notificacao notificacao)
        {
            await _context.Notificacoes.AddAsync(notificacao);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notificacao notificacao)
        {
            _context.Notificacoes.Update(notificacao);
            await _context.SaveChangesAsync();
        }
    }
}
