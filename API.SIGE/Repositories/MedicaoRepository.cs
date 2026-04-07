using API.SIGE.Data;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Repositories
{
    public class MedicaoRepository : IMedicaoRepository
    {
        private readonly AppDbData _context;

        public MedicaoRepository(AppDbData context)
        {
            _context = context;
        }

        public async Task<List<Medicao>> GetAllAsync()
        {
            return await _context.Medicoes
                .Include(m => m.FamiliaCaixilho)
                .Include(m => m.Responsavel)
                .ToListAsync();
        }

        public async Task<Medicao?> GetByIdAsync(int id)
        {
            return await _context.Medicoes
                .Include(m => m.FamiliaCaixilho)
                .Include(m => m.Responsavel)
                .FirstOrDefaultAsync(m => m.IdMedicao == id);
        }

        public async Task<Medicao?> GetByFamiliaIdAsync(int familiaId)
        {
            return await _context.Medicoes
                .Include(m => m.FamiliaCaixilho)
                .Include(m => m.Responsavel)
                .FirstOrDefaultAsync(m => m.IdFamiliaCaixilho == familiaId);
        }

        public async Task AddAsync(Medicao medicao)
        {
            await _context.Medicoes.AddAsync(medicao);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Medicao medicao)
        {
            _context.Medicoes.Update(medicao);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var medicao = await _context.Medicoes.FindAsync(id);
            if (medicao != null)
            {
                _context.Medicoes.Remove(medicao);
                await _context.SaveChangesAsync();
            }
        }
    }
}
