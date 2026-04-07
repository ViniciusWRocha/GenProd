using API.SIGE.Data;
using API.SIGE.DTOs;
using API.SIGE.Interfaces.Services;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbData _context;

        public DashboardService(AppDbData context)
        {
            _context = context;
        }

        public async Task<DashboardMetricasDto> GetMetricasAsync()
        {
            var obras = await _context.Obras.ToListAsync();

            var obrasEmAndamento = obras.Count(o =>
                o.StatusObra != StatusObra.Cadastrada &&
                o.StatusObra != StatusObra.Concluida);

            var obrasProgresso = obras
                .Where(o => !o.Finalizado)
                .Select(o => new ObraDashboardDto
                {
                    IdObra = o.IdObra,
                    Nome = o.Nome,
                    StatusObra = o.StatusObra.ToString(),
                    PercentualMedicao = o.PercentualMedicao,
                    PercentualProducao = o.PercentualProducao
                })
                .ToList();

            return new DashboardMetricasDto
            {
                TotalObras = obras.Count,
                TotalCaixilhos = await _context.Caixilhos.CountAsync(),
                TotalUsuarios = await _context.Usuarios.CountAsync(u => u.Ativo),
                ObrasEmAndamento = obrasEmAndamento,
                ObrasProgresso = obrasProgresso
            };
        }

        public async Task AtualizarProgressoObrasAsync()
        {
            var obras = await _context.Obras.ToListAsync();
            foreach (var obra in obras)
            {
                var familias = await _context.FamiliaCaixilhos
                    .Where(f => f.IdObra == obra.IdObra)
                    .ToListAsync();

                var total = familias.Count;
                if (total > 0)
                {
                    var medidas = familias.Count(f =>
                        f.StatusFamilia == StatusFamilia.Medida ||
                        f.StatusFamilia == StatusFamilia.EmProducao ||
                        f.StatusFamilia == StatusFamilia.Produzida);
                    var produzidas = familias.Count(f => f.StatusFamilia == StatusFamilia.Produzida);

                    obra.PercentualMedicao = (float)medidas / total * 100;
                    obra.PercentualProducao = (float)produzidas / total * 100;
                }

                // Atualizar peso produzido baseado em caixilhos liberados
                var pesoProduzido = await _context.Caixilhos
                    .Where(c => c.ObraId == obra.IdObra && c.Liberado)
                    .SumAsync(c => c.PesoUnitario * c.Quantidade);

                obra.PesoProduzido = pesoProduzido;
                obra.PercentualConclusao = obra.PesoFinal > 0
                    ? Math.Min(100, (pesoProduzido / obra.PesoFinal) * 100)
                    : 0;
            }

            await _context.SaveChangesAsync();
        }
    }
}
