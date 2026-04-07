using API.SIGE.Data;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbData _context;

        public UsuarioRepository(AppDbData context)
        {
            _context = context;
        }

        public async Task AddAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Usuario>> GetAllAtivosAsync()
        {
            return await _context.Usuarios
                .Where(u => u.Ativo)
                .Include(u => u.TipoUsuario)
                .Include(u => u.UsuarioCargos!)
                    .ThenInclude(uc => uc.Cargo)
                .ToListAsync();
        }

        public async Task<List<Usuario>> GetAllInativosAsync()
        {
            return await _context.Usuarios
                .Where(u => u.Ativo == false)
                .Include(u => u.TipoUsuario)
                .Include(u => u.UsuarioCargos!)
                    .ThenInclude(uc => uc.Cargo)
                .ToListAsync();
        }

        public async Task InativarAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null && usuario.Ativo)
            {
                usuario.Ativo = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReativarAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null && !usuario.Ativo)
            {
                usuario.Ativo = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .Include(u => u.UsuarioCargos!)
                    .ThenInclude(uc => uc.Cargo)
                .ToListAsync();
        }

        public async Task<Usuario?> GetById(int id)
        {
            return await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .Include(u => u.UsuarioCargos!)
                    .ThenInclude(uc => uc.Cargo)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .Include(u => u.UsuarioCargos!)
                    .ThenInclude(uc => uc.Cargo)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<Usuario>> GetByCargoAsync(TipoCargo tipoCargo)
        {
            return await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .Include(u => u.UsuarioCargos!)
                    .ThenInclude(uc => uc.Cargo)
                .Where(u => u.UsuarioCargos!.Any(uc => uc.Cargo!.TipoCargo == tipoCargo))
                .ToListAsync();
        }
    }
}
