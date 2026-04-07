using API.SIGE.Data;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Repositories
{
    public class CargoRepository : ICargoRepository
    {
        private readonly AppDbData _context;

        public CargoRepository(AppDbData context)
        {
            _context = context;
        }

        public async Task<List<Cargo>> GetAllAsync()
        {
            return await _context.Cargos.ToListAsync();
        }

        public async Task<Cargo?> GetByIdAsync(int id)
        {
            return await _context.Cargos.FindAsync(id);
        }

        public async Task<Cargo?> GetByTipoAsync(TipoCargo tipo)
        {
            return await _context.Cargos.FirstOrDefaultAsync(c => c.TipoCargo == tipo);
        }

        public async Task AddAsync(Cargo cargo)
        {
            await _context.Cargos.AddAsync(cargo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cargo cargo)
        {
            _context.Cargos.Update(cargo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cargo = await _context.Cargos.FindAsync(id);
            if (cargo != null)
            {
                _context.Cargos.Remove(cargo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
