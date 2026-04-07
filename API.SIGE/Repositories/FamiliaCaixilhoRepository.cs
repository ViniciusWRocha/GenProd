using API.SIGE.Data;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Repositories
{
    public class FamiliaCaixilhoRepository : IFamiliaCaixilhoRepository
    {
        private readonly AppDbData _context;

        public FamiliaCaixilhoRepository(AppDbData context)
        {
            _context = context;
        }

        public async Task AddAsync(FamiliaCaixilho familiaCaixilho)
        {
            await _context.FamiliaCaixilhos.AddAsync(familiaCaixilho);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var familiaCaixilho = await _context.FamiliaCaixilhos.FindAsync(id);
            if (familiaCaixilho != null)
            {
                _context.FamiliaCaixilhos.Remove(familiaCaixilho);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<FamiliaCaixilho>> GetAllAsync()
        {
            return await _context.FamiliaCaixilhos
                .Include(f => f.Obra)
                .ToListAsync();
        }

        public async Task<FamiliaCaixilho?> GetByIdAsync(int id)
        {
            return await _context.FamiliaCaixilhos
                .Include(f => f.Obra)
                .FirstOrDefaultAsync(f => f.IdFamiliaCaixilho == id);
        }

        public async Task<List<FamiliaCaixilho>> GetByObraIdAsync(int obraId)
        {
            return await _context.FamiliaCaixilhos
                .Include(f => f.Obra)
                .Where(f => f.IdObra == obraId)
                .ToListAsync();
        }

        public async Task UpdateAsync(FamiliaCaixilho familiaCaixilho)
        {
            _context.FamiliaCaixilhos.Update(familiaCaixilho);
            await _context.SaveChangesAsync();
        }
    }
}
