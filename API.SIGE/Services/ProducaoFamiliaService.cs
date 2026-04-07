using API.SIGE.Data;
using API.SIGE.DTOs;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Interfaces.Services;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Services
{
    public class ProducaoFamiliaService : IProducaoFamiliaService
    {
        private readonly IProducaoFamiliaRepository _producaoRepository;
        private readonly IFamiliaCaixilhoRepository _familiaRepository;
        private readonly IObraRepository _obraRepository;
        private readonly INotificacaoService _notificacaoService;
        private readonly AppDbData _context;

        public ProducaoFamiliaService(
            IProducaoFamiliaRepository producaoRepository,
            IFamiliaCaixilhoRepository familiaRepository,
            IObraRepository obraRepository,
            INotificacaoService notificacaoService,
            AppDbData context)
        {
            _producaoRepository = producaoRepository;
            _familiaRepository = familiaRepository;
            _obraRepository = obraRepository;
            _notificacaoService = notificacaoService;
            _context = context;
        }

        public async Task<ProducaoFamiliaResponseDto?> GetByIdAsync(int id)
        {
            var producao = await _producaoRepository.GetByIdAsync(id);
            return producao == null ? null : MapToDto(producao);
        }

        public async Task<ProducaoFamiliaResponseDto?> GetByFamiliaIdAsync(int familiaId)
        {
            var producao = await _producaoRepository.GetByFamiliaIdAsync(familiaId);
            return producao == null ? null : MapToDto(producao);
        }

        public async Task<ProducaoFamiliaResponseDto> IniciarAsync(int familiaId, ProducaoFamiliaIniciarDto dto)
        {
            var familia = await _familiaRepository.GetByIdAsync(familiaId);
            if (familia == null)
                throw new InvalidOperationException($"FamiliaCaixilho com ID {familiaId} não encontrada.");

            if (familia.StatusFamilia != StatusFamilia.Medida)
                throw new InvalidOperationException($"Família deve estar com status Medida para iniciar produção. Status atual: {familia.StatusFamilia}");

            var producao = new ProducaoFamilia
            {
                IdFamiliaCaixilho = familiaId,
                IdResponsavel = dto.IdResponsavel,
                Status = StatusAtividade.EmAndamento,
                DataInicio = DateTime.UtcNow,
                DataEstimadaConclusao = dto.DataEstimadaConclusao.HasValue ? DateTime.SpecifyKind(dto.DataEstimadaConclusao.Value, DateTimeKind.Utc) : null,
                Descricao = dto.Descricao,
                Observacoes = dto.Observacoes
            };

            await _producaoRepository.AddAsync(producao);

            familia.StatusFamilia = StatusFamilia.EmProducao;
            await _familiaRepository.UpdateAsync(familia);

            var obra = await _obraRepository.GetById(familia.IdObra);
            if (obra != null && (obra.StatusObra == StatusObra.Verificada || obra.StatusObra == StatusObra.EmMedicao))
            {
                obra.StatusObra = StatusObra.EmProducao;
                await _obraRepository.UpdateAsync(obra);
            }

            var result = await _producaoRepository.GetByIdAsync(producao.IdProducaoFamilia);
            return MapToDto(result!);
        }

        public async Task PausarAsync(int familiaId, ProducaoFamiliaPausarDto dto)
        {
            var producao = await _producaoRepository.GetByFamiliaIdAsync(familiaId);
            if (producao == null)
                throw new InvalidOperationException($"Produção não encontrada para família {familiaId}.");

            if (producao.Status != StatusAtividade.EmAndamento)
                throw new InvalidOperationException("Produção deve estar Em Andamento para pausar.");

            producao.Status = StatusAtividade.Pausada;
            if (dto.Observacoes != null)
                producao.Observacoes = dto.Observacoes;

            await _producaoRepository.UpdateAsync(producao);
        }

        public async Task<ProducaoFamiliaResponseDto> FinalizarAsync(int familiaId, ProducaoFamiliaFinalizarDto dto)
        {
            var producao = await _producaoRepository.GetByFamiliaIdAsync(familiaId);
            if (producao == null)
                throw new InvalidOperationException($"Produção não encontrada para família {familiaId}.");

            if (producao.Status != StatusAtividade.EmAndamento && producao.Status != StatusAtividade.Pausada)
                throw new InvalidOperationException("Produção deve estar Em Andamento ou Pausada para finalizar.");

            producao.Status = StatusAtividade.Concluida;
            producao.DataConclusao = DateTime.UtcNow;
            if (dto.Observacoes != null)
                producao.Observacoes = dto.Observacoes;

            await _producaoRepository.UpdateAsync(producao);

            var familia = await _familiaRepository.GetByIdAsync(familiaId);
            if (familia != null)
            {
                familia.StatusFamilia = StatusFamilia.Produzida;
                await _familiaRepository.UpdateAsync(familia);

                var obra = await _obraRepository.GetById(familia.IdObra);
                if (obra != null)
                {
                    var familias = await _familiaRepository.GetByObraIdAsync(obra.IdObra);
                    var total = familias.Count;
                    var produzidas = familias.Count(f => f.StatusFamilia == StatusFamilia.Produzida);

                    obra.PercentualProducao = total > 0 ? (float)produzidas / total * 100 : 0;
                    await _obraRepository.UpdateAsync(obra);

                    // Se medição e produção estão 100%, notificar gerente
                    if (obra.PercentualMedicao >= 100 && obra.PercentualProducao >= 100)
                    {
                        // Buscar gerentes via UsuarioCargo
                        var gerentes = await _context.UsuarioCargos
                            .Include(uc => uc.Usuario)
                            .Where(uc => uc.Cargo!.TipoCargo == TipoCargo.Gerente)
                            .Select(uc => uc.IdUsuario)
                            .ToListAsync();

                        foreach (var gerenteId in gerentes)
                        {
                            await _notificacaoService.CriarAsync(
                                gerenteId,
                                "Obra pronta para conclusão",
                                $"A obra '{obra.Nome}' atingiu 100% de medição e produção.",
                                TipoNotificacao.ObraConcluida,
                                obra.IdObra);
                        }
                    }
                }
            }

            var result = await _producaoRepository.GetByIdAsync(producao.IdProducaoFamilia);
            return MapToDto(result!);
        }

        private static ProducaoFamiliaResponseDto MapToDto(ProducaoFamilia p)
        {
            return new ProducaoFamiliaResponseDto
            {
                IdProducaoFamilia = p.IdProducaoFamilia,
                IdFamiliaCaixilho = p.IdFamiliaCaixilho,
                DescricaoFamilia = p.FamiliaCaixilho?.DescricaoFamilia,
                IdResponsavel = p.IdResponsavel,
                NomeResponsavel = p.Responsavel?.NomeUsuario,
                Status = p.Status.ToString(),
                DataInicio = p.DataInicio,
                DataEstimadaConclusao = p.DataEstimadaConclusao,
                DataConclusao = p.DataConclusao,
                Descricao = p.Descricao,
                Observacoes = p.Observacoes
            };
        }
    }
}
