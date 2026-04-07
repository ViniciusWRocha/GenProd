using API.SIGE.Data;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Repositories
{
    public class ObraRepository : IObraRepository
    {
        private readonly AppDbData _context;

        public ObraRepository(AppDbData context)
        {
            _context = context;
        }

        public async Task AddAsync(Obra obra)
        {
            await _context.Obras.AddAsync(obra);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var obra = await _context.Obras.FindAsync(id);
            if (obra != null)
            {
                _context.Obras.Remove(obra);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Obra>> GetAllAsync()
        {
            return await _context.Obras
                .Include(o => o.Usuario)
                .ToListAsync();
        }

        public async Task<List<Obra>> GetAllFinalizadosAsync()
        {
            return await _context.Obras
                .Include(o => o.Usuario)
                .Where(o => o.Finalizado)
                .ToListAsync();
        }

        public async Task<List<Obra>> GetAllNaoFinalizadosAsync()
        {
            return await _context.Obras
                .Include(o => o.Usuario)
                .Where(o => o.Finalizado == false)
                .ToListAsync();
        }

        public async Task<Obra?> GetById(int id)
        {
            return await _context.Obras
                .Include(o => o.Usuario)
                .Include(o => o.ResponsavelVerificacao)
                .Include(o => o.ResponsavelMedicao)
                .Include(o => o.ResponsavelProducao)
                .FirstOrDefaultAsync(o => o.IdObra == id);
        }

        public async Task UpdateAsync(Obra obra)
        {
            _context.Obras.Update(obra);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Obra>> GetByStatusAsync(StatusObra status)
        {
            return await _context.Obras
                .Include(o => o.Usuario)
                .Where(o => o.StatusObra == status)
                .ToListAsync();
        }
    }
}
