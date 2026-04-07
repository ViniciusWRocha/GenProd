using API.SIGE.Data;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Repositories
{
    public class TipoUsuarioRepository : ITipoUsuarioRepository
    {
        private readonly AppDbData _context;

        public TipoUsuarioRepository(AppDbData context)
        {
            _context = context;
        }

        public async Task AddAsync(TipoUsuario tipoUsuario)
        {
            await _context.TiposUsuario.AddAsync(tipoUsuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tipoUsuario = await _context.TiposUsuario.FindAsync(id);
            if (tipoUsuario != null)
            {
                _context.TiposUsuario.Remove(tipoUsuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<TipoUsuario>> GetAllAsync()
        {
            return await _context.TiposUsuario.ToListAsync();
        }

        public async Task<TipoUsuario?> GetById(int id)
        {
            return await _context.TiposUsuario.FindAsync(id);
        }

        public async Task UpdateAsync(TipoUsuario tipoUsuario)
        {
            _context.TiposUsuario.Update(tipoUsuario);
            await _context.SaveChangesAsync();
        }
    }
}
