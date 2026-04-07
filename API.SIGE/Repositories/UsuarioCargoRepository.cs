using API.SIGE.Data;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Repositories
{
    public class UsuarioCargoRepository : IUsuarioCargoRepository
    {
        private readonly AppDbData _context;

        public UsuarioCargoRepository(AppDbData context)
        {
            _context = context;
        }

        public async Task<List<UsuarioCargo>> GetByUsuarioIdAsync(int idUsuario)
        {
            return await _context.UsuarioCargos
                .Include(uc => uc.Cargo)
                .Where(uc => uc.IdUsuario == idUsuario)
                .ToListAsync();
        }

        public async Task AddAsync(UsuarioCargo usuarioCargo)
        {
            await _context.UsuarioCargos.AddAsync(usuarioCargo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int idUsuario, int idCargo)
        {
            var uc = await _context.UsuarioCargos
                .FirstOrDefaultAsync(x => x.IdUsuario == idUsuario && x.IdCargo == idCargo);
            if (uc != null)
            {
                _context.UsuarioCargos.Remove(uc);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int idUsuario, int idCargo)
        {
            return await _context.UsuarioCargos
                .AnyAsync(x => x.IdUsuario == idUsuario && x.IdCargo == idCargo);
        }
    }
}
