using API.SIGE.Data;
using API.SIGE.DTOs;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Interfaces.Services;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;

namespace API.SIGE.Services
{
    public class MedicaoService : IMedicaoService
    {
        private readonly IMedicaoRepository _medicaoRepository;
        private readonly IFamiliaCaixilhoRepository _familiaRepository;
        private readonly IObraRepository _obraRepository;
        private readonly INotificacaoService _notificacaoService;
        private readonly AppDbData _context;

        public MedicaoService(
            IMedicaoRepository medicaoRepository,
            IFamiliaCaixilhoRepository familiaRepository,
            IObraRepository obraRepository,
            INotificacaoService notificacaoService,
            AppDbData context)
        {
            _medicaoRepository = medicaoRepository;
            _familiaRepository = familiaRepository;
            _obraRepository = obraRepository;
            _notificacaoService = notificacaoService;
            _context = context;
        }

        public async Task<MedicaoResponseDto?> GetByIdAsync(int id)
        {
            var medicao = await _medicaoRepository.GetByIdAsync(id);
            return medicao == null ? null : MapToDto(medicao);
        }

        public async Task<MedicaoResponseDto?> GetByFamiliaIdAsync(int familiaId)
        {
            var medicao = await _medicaoRepository.GetByFamiliaIdAsync(familiaId);
            return medicao == null ? null : MapToDto(medicao);
        }

        public async Task<MedicaoResponseDto> IniciarAsync(int familiaId, MedicaoIniciarDto dto)
        {
            var familia = await _familiaRepository.GetByIdAsync(familiaId);
            if (familia == null)
                throw new InvalidOperationException($"FamiliaCaixilho com ID {familiaId} não encontrada.");

            if (familia.StatusFamilia != StatusFamilia.Pendente)
                throw new InvalidOperationException($"Família deve estar com status Pendente para iniciar medição. Status atual: {familia.StatusFamilia}");

            var medicao = new Medicao
            {
                IdFamiliaCaixilho = familiaId,
                IdResponsavel = dto.IdResponsavel,
                Status = StatusAtividade.EmAndamento,
                DataInicio = DateTime.UtcNow,
                DataEstimadaConclusao = dto.DataEstimadaConclusao.HasValue ? DateTime.SpecifyKind(dto.DataEstimadaConclusao.Value, DateTimeKind.Utc) : null,
                Descricao = dto.Descricao,
                Observacoes = dto.Observacoes
            };

            await _medicaoRepository.AddAsync(medicao);

            familia.StatusFamilia = StatusFamilia.EmMedicao;
            await _familiaRepository.UpdateAsync(familia);

            var obra = await _obraRepository.GetById(familia.IdObra);
            if (obra != null && obra.StatusObra == StatusObra.Verificada)
            {
                obra.StatusObra = StatusObra.EmMedicao;
                await _obraRepository.UpdateAsync(obra);
            }

            var result = await _medicaoRepository.GetByIdAsync(medicao.IdMedicao);
            return MapToDto(result!);
        }

        public async Task PausarAsync(int familiaId, MedicaoPausarDto dto)
        {
            var medicao = await _medicaoRepository.GetByFamiliaIdAsync(familiaId);
            if (medicao == null)
                throw new InvalidOperationException($"Medição não encontrada para família {familiaId}.");

            if (medicao.Status != StatusAtividade.EmAndamento)
                throw new InvalidOperationException("Medição deve estar Em Andamento para pausar.");

            medicao.Status = StatusAtividade.Pausada;
            if (dto.Observacoes != null)
                medicao.Observacoes = dto.Observacoes;

            await _medicaoRepository.UpdateAsync(medicao);
        }

        public async Task<MedicaoResponseDto> FinalizarAsync(int familiaId, MedicaoFinalizarDto dto)
        {
            var medicao = await _medicaoRepository.GetByFamiliaIdAsync(familiaId);
            if (medicao == null)
                throw new InvalidOperationException($"Medição não encontrada para família {familiaId}.");

            if (medicao.Status != StatusAtividade.EmAndamento && medicao.Status != StatusAtividade.Pausada)
                throw new InvalidOperationException("Medição deve estar Em Andamento ou Pausada para finalizar.");

            medicao.Status = StatusAtividade.Concluida;
            medicao.DataConclusao = DateTime.UtcNow;
            if (dto.Observacoes != null)
                medicao.Observacoes = dto.Observacoes;

            await _medicaoRepository.UpdateAsync(medicao);

            var familia = await _familiaRepository.GetByIdAsync(familiaId);
            if (familia != null)
            {
                familia.StatusFamilia = StatusFamilia.Medida;
                await _familiaRepository.UpdateAsync(familia);

                // Recalcular percentual de medição da obra
                var obra = await _obraRepository.GetById(familia.IdObra);
                if (obra != null)
                {
                    var familias = await _familiaRepository.GetByObraIdAsync(obra.IdObra);
                    var total = familias.Count;
                    var medidas = familias.Count(f =>
                        f.StatusFamilia == StatusFamilia.Medida ||
                        f.StatusFamilia == StatusFamilia.EmProducao ||
                        f.StatusFamilia == StatusFamilia.Produzida);

                    obra.PercentualMedicao = total > 0 ? (float)medidas / total * 100 : 0;
                    await _obraRepository.UpdateAsync(obra);

                    // Notificar responsável de produção
                    if (obra.IdResponsavelProducao.HasValue)
                    {
                        await _notificacaoService.CriarAsync(
                            obra.IdResponsavelProducao.Value,
                            "Família medida",
                            $"A família '{familia.DescricaoFamilia}' da obra '{obra.Nome}' foi medida e está pronta para produção.",
                            TipoNotificacao.FamiliaMedida,
                            obra.IdObra);
                    }
                }
            }

            var result = await _medicaoRepository.GetByIdAsync(medicao.IdMedicao);
            return MapToDto(result!);
        }

        private static MedicaoResponseDto MapToDto(Medicao m)
        {
            return new MedicaoResponseDto
            {
                IdMedicao = m.IdMedicao,
                IdFamiliaCaixilho = m.IdFamiliaCaixilho,
                DescricaoFamilia = m.FamiliaCaixilho?.DescricaoFamilia,
                IdResponsavel = m.IdResponsavel,
                NomeResponsavel = m.Responsavel?.NomeUsuario,
                Status = m.Status.ToString(),
                DataInicio = m.DataInicio,
                DataEstimadaConclusao = m.DataEstimadaConclusao,
                DataConclusao = m.DataConclusao,
                Descricao = m.Descricao,
                Observacoes = m.Observacoes
            };
        }
    }
}
