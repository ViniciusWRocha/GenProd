using API.SIGE.Data;
using API.SIGE.DTOs;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Interfaces.Services;
using API.SIGE.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace API.SIGE.Services
{
    public class ObraService : IObraService
    {
        private readonly IObraRepository _obraRepository;
        private readonly IFamiliaCaixilhoRepository _familiaRepository;
        private readonly INotificacaoService _notificacaoService;
        private readonly AppDbData _context;

        public ObraService(
            IObraRepository obraRepository,
            IFamiliaCaixilhoRepository familiaRepository,
            INotificacaoService notificacaoService,
            AppDbData context)
        {
            _obraRepository = obraRepository;
            _familiaRepository = familiaRepository;
            _notificacaoService = notificacaoService;
            _context = context;
        }

        public async Task<List<ObraResponseDto>> GetAllAsync(bool? finalizadas)
        {
            var obras = finalizadas switch
            {
                true => await _obraRepository.GetAllFinalizadosAsync(),
                false => await _obraRepository.GetAllNaoFinalizadosAsync(),
                _ => await _obraRepository.GetAllAsync()
            };

            return obras
                .OrderByDescending(o => o.IdObra)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<ObraResponseDto?> GetByIdAsync(int id)
        {
            var obra = await _obraRepository.GetById(id);
            return obra == null ? null : MapToDto(obra);
        }

        public async Task<ObraResponseDto> CreateAsync(ObraCreateDto dto)
        {
            var obra = new Obra
            {
                Nome = dto.Nome,
                Construtora = dto.Construtora,
                Nro = dto.Nro,
                Logradouro = dto.Logradouro,
                Bairro = dto.Bairro,
                Cep = dto.Cep,
                Uf = dto.Uf,
                Cnpj = dto.Cnpj,
                DataInicio = DateTime.SpecifyKind(dto.DataInicio, DateTimeKind.Utc),
                DataTermino = DateTime.SpecifyKind(dto.DataTermino, DateTimeKind.Utc),
                PesoFinal = 0,
                Observacoes = dto.Observacoes,
                IdUsuario = dto.IdUsuario,
                StatusObra = StatusObra.Cadastrada,
                IdResponsavelVerificacao = dto.IdResponsavelVerificacao,
                IdResponsavelMedicao = dto.IdResponsavelMedicao,
                IdResponsavelProducao = dto.IdResponsavelProducao
            };

            await _obraRepository.AddAsync(obra);
            var result = await _obraRepository.GetById(obra.IdObra);
            return MapToDto(result!);
        }

        public async Task UpdateAsync(int id, ObraUpdateDto dto)
        {
            var obra = await _obraRepository.GetById(id);
            if (obra == null)
                throw new InvalidOperationException($"Obra com ID {id} não encontrada.");

            obra.Nome = dto.Nome;
            obra.Construtora = dto.Construtora;
            obra.Nro = dto.Nro;
            obra.Logradouro = dto.Logradouro;
            obra.Bairro = dto.Bairro;
            obra.Cep = dto.Cep;
            obra.Uf = dto.Uf;
            obra.Cnpj = dto.Cnpj;
            obra.DataInicio = DateTime.SpecifyKind(dto.DataInicio, DateTimeKind.Utc);
            obra.DataTermino = DateTime.SpecifyKind(dto.DataTermino, DateTimeKind.Utc);
            // PesoFinal é calculado automaticamente (soma das famílias)
            obra.Observacoes = dto.Observacoes;
            if (!string.IsNullOrEmpty(dto.StatusObra) && Enum.TryParse<StatusObra>(dto.StatusObra, out var novoStatus))
            {
                obra.StatusObra = novoStatus;
            }
            obra.Finalizado = dto.Finalizado;
            obra.ImagemObraPath = dto.ImagemObraPath;
            obra.IdUsuario = dto.IdUsuario;
            obra.IdResponsavelVerificacao = dto.IdResponsavelVerificacao;
            obra.IdResponsavelMedicao = dto.IdResponsavelMedicao;
            obra.IdResponsavelProducao = dto.IdResponsavelProducao;

            await _obraRepository.UpdateAsync(obra);
        }

        public async Task DeleteAsync(int id)
        {
            await _obraRepository.DeleteAsync(id);
        }

        public async Task<List<string>> ImportarObrasXmlAsync(List<IFormFile> arquivosXml)
        {
            var resultados = new List<string>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            foreach (var arquivoXml in arquivosXml)
            {
                try
                {
                    using var stream = arquivoXml.OpenReadStream();
                    using var reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("windows-1252"));
                    var xdoc = XDocument.Load(reader);

                    var dadosObra = xdoc.Root?.Element("DADOS_OBRA");
                    var enderecoObra = dadosObra?.Element("ENDERECO_OBRA");
                    var dadosCliente = xdoc.Root?.Element("DADOS_CLIENTE");

                    var obra = new Obra
                    {
                        Nome = dadosObra?.Element("NOME")?.Value ?? "Obra sem nome",
                        Construtora = dadosCliente?.Element("NOME")?.Value ?? "Sem construtora",
                        Cnpj = dadosCliente?.Element("CNPJ_CPF")?.Value ?? "00000000000000",
                        Logradouro = enderecoObra?.Element("END_LOGR")?.Value ?? "Nao informado",
                        Nro = enderecoObra?.Element("END_NUMERO")?.Value ?? "0",
                        Bairro = enderecoObra?.Element("END_BAIRRO")?.Value ?? "Nao informado",
                        Cep = enderecoObra?.Element("END_CEP")?.Value ?? "00000-000",
                        Uf = enderecoObra?.Element("END_UF")?.Value ?? "SP",
                        IdUsuario = 3,
                        StatusObra = StatusObra.Cadastrada
                    };

                    _context.Obras.Add(obra);
                    resultados.Add($"Arquivo '{arquivoXml.FileName}' importado com sucesso.");
                }
                catch (Exception ex)
                {
                    resultados.Add($"Arquivo '{arquivoXml.FileName}' com erro: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
            return resultados;
        }

        public async Task VerificarAsync(int id)
        {
            var obra = await _obraRepository.GetById(id);
            if (obra == null)
                throw new InvalidOperationException($"Obra com ID {id} não encontrada.");

            if (obra.StatusObra != StatusObra.Cadastrada)
                throw new InvalidOperationException($"Obra deve estar Cadastrada para ser verificada. Status atual: {obra.StatusObra}");

            obra.StatusObra = StatusObra.Verificada;
            await _obraRepository.UpdateAsync(obra);

            // Notificar responsável de medição
            if (obra.IdResponsavelMedicao.HasValue)
            {
                await _notificacaoService.CriarAsync(
                    obra.IdResponsavelMedicao.Value,
                    "Obra verificada",
                    $"A obra '{obra.Nome}' foi verificada e está pronta para medição.",
                    TipoNotificacao.ObraVerificada,
                    obra.IdObra);
            }
        }

        public async Task ConcluirAsync(int id)
        {
            var obra = await _obraRepository.GetById(id);
            if (obra == null)
                throw new InvalidOperationException($"Obra com ID {id} não encontrada.");

            if (obra.PercentualMedicao < 100 || obra.PercentualProducao < 100)
                throw new InvalidOperationException("Obra deve ter 100% de medição e 100% de produção para ser concluída.");

            obra.StatusObra = StatusObra.Concluida;
            obra.Finalizado = true;
            obra.DataConclusao = DateTime.UtcNow;
            await _obraRepository.UpdateAsync(obra);
        }

        public async Task RecalcularProgressoAsync(int id)
        {
            var obra = await _obraRepository.GetById(id);
            if (obra == null)
                throw new InvalidOperationException($"Obra com ID {id} não encontrada.");

            var familias = await _familiaRepository.GetByObraIdAsync(id);

            // PercentualMedicao: baseado em caixilhos individuais
            var todosCaixilhos = await _context.Caixilhos
                .Where(c => c.ObraId == id)
                .ToListAsync();
            var totalCaixilhos = todosCaixilhos.Count;

            if (totalCaixilhos > 0)
            {
                var caixilhosMedidos = todosCaixilhos.Count(c =>
                    c.StatusProducao == StatusProducao.Medido ||
                    c.StatusProducao == StatusProducao.Concluido);
                obra.PercentualMedicao = (float)caixilhosMedidos / totalCaixilhos * 100;
            }
            else
            {
                obra.PercentualMedicao = 0;
            }

            // PercentualProducao: baseado em famílias liberadas (EmProducao ou Produzida)
            var totalFamilias = familias.Count;
            if (totalFamilias > 0)
            {
                var familiasLiberadas = familias.Count(f =>
                    f.StatusFamilia == StatusFamilia.EmProducao ||
                    f.StatusFamilia == StatusFamilia.Produzida);
                obra.PercentualProducao = (float)familiasLiberadas / totalFamilias * 100;
            }
            else
            {
                obra.PercentualProducao = 0;
            }

            // Atualizar StatusObra automaticamente
            if (obra.StatusObra != StatusObra.Concluida)
            {
                if (obra.PercentualProducao > 0)
                    obra.StatusObra = StatusObra.EmProducao;
                else if (obra.PercentualMedicao > 0)
                    obra.StatusObra = StatusObra.EmMedicao;
            }

            await _obraRepository.UpdateAsync(obra);
        }

        private static ObraResponseDto MapToDto(Obra obra)
        {
            return new ObraResponseDto
            {
                IdObra = obra.IdObra,
                Nome = obra.Nome,
                Construtora = obra.Construtora,
                Nro = obra.Nro,
                Logradouro = obra.Logradouro,
                Bairro = obra.Bairro,
                Cep = obra.Cep,
                Uf = obra.Uf,
                Cnpj = obra.Cnpj,
                DataInicio = obra.DataInicio,
                DataTermino = obra.DataTermino,
                PesoFinal = obra.PesoFinal,
                PesoProduzido = obra.PesoProduzido,
                PercentualConclusao = obra.PercentualConclusao,
                DataConclusao = obra.DataConclusao,
                Observacoes = obra.Observacoes,
                Finalizado = obra.Finalizado,
                ImagemObraPath = obra.ImagemObraPath,
                IdUsuario = obra.IdUsuario,
                NomeUsuario = obra.Usuario?.NomeUsuario,
                StatusObra = obra.StatusObra.ToString(),
                PercentualMedicao = obra.PercentualMedicao,
                PercentualProducao = obra.PercentualProducao,
                IdResponsavelVerificacao = obra.IdResponsavelVerificacao,
                NomeResponsavelVerificacao = obra.ResponsavelVerificacao?.NomeUsuario,
                IdResponsavelMedicao = obra.IdResponsavelMedicao,
                NomeResponsavelMedicao = obra.ResponsavelMedicao?.NomeUsuario,
                IdResponsavelProducao = obra.IdResponsavelProducao,
                NomeResponsavelProducao = obra.ResponsavelProducao?.NomeUsuario
            };
        }
    }
}
