using API.SIGE.Data;
using API.SIGE.DTOs;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Interfaces.Services;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Services
{
    public class CaixilhoService : ICaixilhoService
    {
        private readonly ICaixilhoRepository _caixilhoRepository;
        private readonly IObraRepository _obraRepository;
        private readonly AppDbData _context;

        public CaixilhoService(
            ICaixilhoRepository caixilhoRepository,
            IObraRepository obraRepository,
            AppDbData context)
        {
            _caixilhoRepository = caixilhoRepository;
            _obraRepository = obraRepository;
            _context = context;
        }

        public async Task<List<CaixilhoResponseDto>> GetAllAsync()
        {
            var caixilhos = await _caixilhoRepository.GetAllAsync();
            return caixilhos.Select(MapToDto).ToList();
        }

        public async Task<CaixilhoResponseDto?> GetByIdAsync(int id)
        {
            var caixilho = await _caixilhoRepository.GetById(id);
            return caixilho == null ? null : MapToDto(caixilho);
        }

        public async Task<CaixilhoResponseDto> CreateAsync(CaixilhoCreateDto dto)
        {
            var obra = await _obraRepository.GetById(dto.ObraId);
            if (obra == null)
                throw new InvalidOperationException("ObraId inválido.");

            var caixilho = new Caixilho
            {
                NomeCaixilho = dto.NomeCaixilho,
                Largura = dto.Largura,
                Altura = dto.Altura,
                Quantidade = dto.Quantidade,
                PesoUnitario = dto.PesoUnitario,
                Observacoes = dto.Observacoes,
                DescricaoCaixilho = dto.DescricaoCaixilho,
                StatusProducao = dto.StatusProducao,
                ObraId = dto.ObraId,
                IdFamiliaCaixilho = dto.IdFamiliaCaixilho
            };

            await _caixilhoRepository.AddAsync(caixilho);
            await AtualizarPesoFamiliaAsync(caixilho.IdFamiliaCaixilho);

            return MapToDto(caixilho);
        }

        public async Task UpdateAsync(int id, CaixilhoUpdateDto dto)
        {
            var caixilhoOriginal = await _caixilhoRepository.GetById(id);
            if (caixilhoOriginal == null)
                throw new InvalidOperationException($"Caixilho com ID {id} não encontrado.");

            var familiaAnterior = caixilhoOriginal.IdFamiliaCaixilho;

            caixilhoOriginal.NomeCaixilho = dto.NomeCaixilho;
            caixilhoOriginal.Largura = dto.Largura;
            caixilhoOriginal.Altura = dto.Altura;
            caixilhoOriginal.Quantidade = dto.Quantidade;
            caixilhoOriginal.PesoUnitario = dto.PesoUnitario;
            caixilhoOriginal.ObraId = dto.ObraId;
            caixilhoOriginal.IdFamiliaCaixilho = dto.IdFamiliaCaixilho;
            caixilhoOriginal.Liberado = dto.Liberado;
            caixilhoOriginal.DataLiberacao = dto.DataLiberacao;
            caixilhoOriginal.StatusProducao = dto.StatusProducao;
            caixilhoOriginal.Observacoes = dto.Observacoes;

            await _caixilhoRepository.UpdateAsync(caixilhoOriginal);
            await AtualizarPesoFamiliaAsync(dto.IdFamiliaCaixilho);

            if (familiaAnterior != dto.IdFamiliaCaixilho)
            {
                await AtualizarPesoFamiliaAsync(familiaAnterior);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var caixilho = await _caixilhoRepository.GetById(id);
            if (caixilho == null)
                throw new InvalidOperationException($"Caixilho com ID {id} não encontrado.");

            var familiaId = caixilho.IdFamiliaCaixilho;
            await _caixilhoRepository.DeleteAsync(id);
            await AtualizarPesoFamiliaAsync(familiaId);
        }

        public async Task LiberarAsync(int id)
        {
            var caixilho = await _caixilhoRepository.GetById(id);
            if (caixilho == null)
                throw new InvalidOperationException($"Caixilho com ID {id} não encontrado.");

            caixilho.Liberado = true;
            caixilho.DataLiberacao = DateTime.UtcNow;
            await _caixilhoRepository.UpdateAsync(caixilho);
        }

        public async Task<List<CaixilhoResponseDto>> GetByFamiliaIdAsync(int familiaId)
        {
            var caixilhos = await _context.Caixilhos
                .Include(c => c.Obra)
                .Include(c => c.FamiliaCaixilho)
                .Where(c => c.IdFamiliaCaixilho == familiaId)
                .ToListAsync();
            return caixilhos.Select(MapToDto).ToList();
        }

        private async Task AtualizarPesoFamiliaAsync(int familiaId)
        {
            var pesoTotal = await _context.Caixilhos
                .Where(c => c.IdFamiliaCaixilho == familiaId)
                .SumAsync(c => c.PesoUnitario * c.Quantidade);

            var familia = await _context.FamiliaCaixilhos.FindAsync(familiaId);
            if (familia != null)
            {
                familia.PesoTotal = (int)pesoTotal;
                familia.QuantidadeCaixilhos = await _context.Caixilhos
                    .CountAsync(c => c.IdFamiliaCaixilho == familiaId);
                await _context.SaveChangesAsync();

                await AtualizarPesoObraAsync(familia.IdObra);
            }
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

        private static CaixilhoResponseDto MapToDto(Caixilho caixilho)
        {
            return new CaixilhoResponseDto
            {
                IdCaixilho = caixilho.IdCaixilho,
                NomeCaixilho = caixilho.NomeCaixilho,
                Largura = caixilho.Largura,
                Altura = caixilho.Altura,
                Quantidade = caixilho.Quantidade,
                PesoUnitario = caixilho.PesoUnitario,
                Liberado = caixilho.Liberado,
                DataLiberacao = caixilho.DataLiberacao,
                Observacoes = caixilho.Observacoes,
                DescricaoCaixilho = caixilho.DescricaoCaixilho,
                StatusProducao = caixilho.StatusProducao,
                ObraId = caixilho.ObraId,
                NomeObra = caixilho.Obra?.Nome,
                IdFamiliaCaixilho = caixilho.IdFamiliaCaixilho,
                DescricaoFamilia = caixilho.FamiliaCaixilho?.DescricaoFamilia
            };
        }
    }
}
