using API.SIGE.Data;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Repositories
{
    public class ProducaoFamiliaRepository : IProducaoFamiliaRepository
    {
        private readonly AppDbData _context;

        public ProducaoFamiliaRepository(AppDbData context)
        {
            _context = context;
        }

        public async Task<List<ProducaoFamilia>> GetAllAsync()
        {
            return await _context.ProducoesFamilia
                .Include(p => p.FamiliaCaixilho)
                .Include(p => p.Responsavel)
                .ToListAsync();
        }

        public async Task<ProducaoFamilia?> GetByIdAsync(int id)
        {
            return await _context.ProducoesFamilia
                .Include(p => p.FamiliaCaixilho)
                .Include(p => p.Responsavel)
                .FirstOrDefaultAsync(p => p.IdProducaoFamilia == id);
        }

        public async Task<ProducaoFamilia?> GetByFamiliaIdAsync(int familiaId)
        {
            return await _context.ProducoesFamilia
                .Include(p => p.FamiliaCaixilho)
                .Include(p => p.Responsavel)
                .FirstOrDefaultAsync(p => p.IdFamiliaCaixilho == familiaId);
        }

        public async Task AddAsync(ProducaoFamilia producaoFamilia)
        {
            await _context.ProducoesFamilia.AddAsync(producaoFamilia);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProducaoFamilia producaoFamilia)
        {
            _context.ProducoesFamilia.Update(producaoFamilia);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var producao = await _context.ProducoesFamilia.FindAsync(id);
            if (producao != null)
            {
                _context.ProducoesFamilia.Remove(producao);
                await _context.SaveChangesAsync();
            }
        }
    }
}
