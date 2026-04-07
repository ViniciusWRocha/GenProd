using API.SIGE.Data;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Repositories
{
    public class AnexoRepository : IAnexoRepository
    {
        private readonly AppDbData _context;

        public AnexoRepository(AppDbData context)
        {
            _context = context;
        }

        public async Task<Anexo?> GetByIdAsync(int id)
        {
            return await _context.Anexos
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.IdAnexo == id);
        }

        public async Task<List<Anexo>> GetByMedicaoIdAsync(int medicaoId)
        {
            return await _context.Anexos
                .Include(a => a.Usuario)
                .Where(a => a.IdMedicao == medicaoId)
                .ToListAsync();
        }

        public async Task<List<Anexo>> GetByProducaoFamiliaIdAsync(int producaoFamiliaId)
        {
            return await _context.Anexos
                .Include(a => a.Usuario)
                .Where(a => a.IdProducaoFamilia == producaoFamiliaId)
                .ToListAsync();
        }

        public async Task AddAsync(Anexo anexo)
        {
            await _context.Anexos.AddAsync(anexo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var anexo = await _context.Anexos.FindAsync(id);
            if (anexo != null)
            {
                _context.Anexos.Remove(anexo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
