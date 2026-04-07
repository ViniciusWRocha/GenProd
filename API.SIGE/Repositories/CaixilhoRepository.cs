using API.SIGE.Data;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Repositories
{
    public class CaixilhoRepository : ICaixilhoRepository
    {
        private readonly AppDbData _context;

        public CaixilhoRepository(AppDbData context)
        {
            _context = context;
        }

        public async Task AddAsync(Caixilho caixilho)
        {
            await _context.Caixilhos.AddAsync(caixilho);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var caixilho = await _context.Caixilhos.FindAsync(id);
            if (caixilho != null)
            {
                _context.Caixilhos.Remove(caixilho);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Caixilho>> GetAllAsync()
        {
            return await _context.Caixilhos
                .Include(c => c.Obra)
                .Include(c => c.FamiliaCaixilho)
                .ToListAsync();
        }

        public async Task<Caixilho?> GetById(int id)
        {
            return await _context.Caixilhos
                .Include(c => c.Obra)
                .Include(c => c.FamiliaCaixilho)
                .FirstOrDefaultAsync(c => c.IdCaixilho == id);
        }

        public async Task<Caixilho?> GetByFamilia(Caixilho caixilho)
        {
            return await _context.Caixilhos
                .Include(c => c.Obra)
                .Include(c => c.FamiliaCaixilho)
                .FirstOrDefaultAsync(c => c.IdFamiliaCaixilho == caixilho.IdFamiliaCaixilho);
        }

        public async Task UpdateAsync(Caixilho caixilho)
        {
            var caixilhoTracked = await _context.Caixilhos.FindAsync(caixilho.IdCaixilho);
            if (caixilhoTracked == null)
            {
                throw new InvalidOperationException($"Caixilho com ID {caixilho.IdCaixilho} não encontrado.");
            }

            caixilhoTracked.NomeCaixilho = caixilho.NomeCaixilho;
            caixilhoTracked.Largura = caixilho.Largura;
            caixilhoTracked.Altura = caixilho.Altura;
            caixilhoTracked.Quantidade = caixilho.Quantidade;
            caixilhoTracked.PesoUnitario = caixilho.PesoUnitario;
            caixilhoTracked.ObraId = caixilho.ObraId;
            caixilhoTracked.IdFamiliaCaixilho = caixilho.IdFamiliaCaixilho;
            caixilhoTracked.Liberado = caixilho.Liberado;
            caixilhoTracked.DataLiberacao = caixilho.DataLiberacao;
            caixilhoTracked.StatusProducao = caixilho.StatusProducao;
            caixilhoTracked.Observacoes = caixilho.Observacoes;

            await _context.SaveChangesAsync();
        }
    }
}
