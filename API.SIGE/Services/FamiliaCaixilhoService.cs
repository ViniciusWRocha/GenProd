using API.SIGE.Data;
using API.SIGE.DTOs;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Interfaces.Services;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Services
{
    public class FamiliaCaixilhoService : IFamiliaCaixilhoService
    {
        private readonly IFamiliaCaixilhoRepository _familiaRepository;
        private readonly AppDbData _context;

        public FamiliaCaixilhoService(IFamiliaCaixilhoRepository familiaRepository, AppDbData context)
        {
            _familiaRepository = familiaRepository;
            _context = context;
        }

        public async Task<List<FamiliaCaixilhoResponseDto>> GetAllAsync()
        {
            var familias = await _familiaRepository.GetAllAsync();
            return await MapToDtoListAsync(familias);
        }

        public async Task<FamiliaCaixilhoResponseDto?> GetByIdAsync(int id)
        {
            var familia = await _familiaRepository.GetByIdAsync(id);
            return familia == null ? null : await MapToDtoAsync(familia);
        }

        public async Task<FamiliaCaixilhoResponseDto> CreateAsync(FamiliaCaixilhoCreateDto dto)
        {
            // Validar max 10 famílias por obra
            var existentes = await _familiaRepository.GetByObraIdAsync(dto.IdObra);
            if (existentes.Count >= 10)
                throw new InvalidOperationException("Limite máximo de 10 famílias por obra atingido.");

            var familia = new FamiliaCaixilho
            {
                DescricaoFamilia = dto.DescricaoFamilia,
                IdObra = dto.IdObra,
                PesoTotal = 0,
                StatusFamilia = StatusFamilia.Pendente
            };

            await _familiaRepository.AddAsync(familia);
            var result = await _familiaRepository.GetByIdAsync(familia.IdFamiliaCaixilho);
            return await MapToDtoAsync(result!);
        }

        public async Task UpdateAsync(int id, FamiliaCaixilhoUpdateDto dto)
        {
            var familia = await _familiaRepository.GetByIdAsync(id);
            if (familia == null)
                throw new InvalidOperationException($"FamiliaCaixilho com ID {id} não encontrada.");

            familia.DescricaoFamilia = dto.DescricaoFamilia;
            if (dto.StatusFamilia.HasValue)
                familia.StatusFamilia = dto.StatusFamilia.Value;
            await _familiaRepository.UpdateAsync(familia);
        }

        public async Task DeleteAsync(int id)
        {
            var familia = await _familiaRepository.GetByIdAsync(id);
            var obraId = familia?.IdObra;

            await _familiaRepository.DeleteAsync(id);

            if (obraId.HasValue)
            {
                await AtualizarPesoObraAsync(obraId.Value);
            }
        }

        public async Task<int> RecalcularPesosAsync()
        {
            var familias = await _familiaRepository.GetAllAsync();
            foreach (var familia in familias)
            {
                var pesoTotal = await _context.Caixilhos
                    .Where(c => c.IdFamiliaCaixilho == familia.IdFamiliaCaixilho)
                    .SumAsync(c => c.PesoUnitario * c.Quantidade);

                familia.PesoTotal = (int)pesoTotal;
            }

            await _context.SaveChangesAsync();

            // Recalcular peso de todas as obras afetadas
            var obraIds = familias.Select(f => f.IdObra).Distinct();
            foreach (var obraId in obraIds)
            {
                await AtualizarPesoObraAsync(obraId);
            }

            return familias.Count;
        }

        public async Task<List<FamiliaCaixilhoResponseDto>> GetByObraIdAsync(int obraId)
        {
            var familias = await _familiaRepository.GetByObraIdAsync(obraId);
            return await MapToDtoListAsync(familias);
        }

        private async Task AtualizarPesoObraAsync(int obraId)
        {
            var pesoTotal = await _context.FamiliaCaixilhos
                .Where(f => f.IdObra == obraId)
                .SumAsync(f => f.PesoTotal);

            var obra = await _context.Obras.FindAsync(obraId);
            if (obra != null)
            {
                obra.PesoFinal = pesoTotal;
                await _context.SaveChangesAsync();
            }
        }

        private async Task<FamiliaCaixilhoResponseDto> MapToDtoAsync(FamiliaCaixilho familia)
        {
            var qtdCaixilhos = await _context.Caixilhos
                .CountAsync(c => c.IdFamiliaCaixilho == familia.IdFamiliaCaixilho);

            return new FamiliaCaixilhoResponseDto
            {
                IdFamiliaCaixilho = familia.IdFamiliaCaixilho,
                DescricaoFamilia = familia.DescricaoFamilia,
                PesoTotal = familia.PesoTotal,
                IdObra = familia.IdObra,
                NomeObra = familia.Obra?.Nome,
                StatusFamilia = familia.StatusFamilia.ToString(),
                QuantidadeCaixilhos = qtdCaixilhos
            };
        }

        private async Task<List<FamiliaCaixilhoResponseDto>> MapToDtoListAsync(List<FamiliaCaixilho> familias)
        {
            var result = new List<FamiliaCaixilhoResponseDto>();
            foreach (var familia in familias)
            {
                result.Add(await MapToDtoAsync(familia));
            }
            return result;
        }
    }
}
