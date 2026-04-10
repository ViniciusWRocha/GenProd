using GerenciamentoProducao.ApiDtos;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Services
{
    public class CaixilhoApiService : ApiClientBase, ICaixilhoRepository
    {
        public CaixilhoApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor) { }

        public async Task<List<Caixilho>> GetAllAsync()
        {
            var dtos = await GetListAsync<CaixilhoResponseDto>("/api/caixilho");
            return dtos.Select(MapToModel).ToList();
        }

        public async Task<Caixilho> GetById(int id)
        {
            var dto = await GetAsync<CaixilhoResponseDto>($"/api/caixilho/{id}");
            return dto != null ? MapToModel(dto) : throw new Exception("Caixilho não encontrado");
        }

        public async Task AddAsync(Caixilho caixilho)
        {
            var dto = new CaixilhoCreateDto
            {
                NomeCaixilho = caixilho.NomeCaixilho,
                Largura = caixilho.Largura,
                Altura = caixilho.Altura,
                Quantidade = caixilho.Quantidade,
                PesoUnitario = caixilho.PesoUnitario,
                Observacoes = caixilho.Observacoes,
                DescricaoCaixilho = caixilho.DescricaoCaixilho,
                ObraId = caixilho.ObraId,
                IdFamiliaCaixilho = caixilho.IdFamiliaCaixilho
            };
            await PostAsync<CaixilhoCreateDto>("/api/caixilho", dto);
        }

        public async Task UpdateAsync(Caixilho caixilho)
        {
            var dto = new CaixilhoUpdateDto
            {
                NomeCaixilho = caixilho.NomeCaixilho,
                Largura = caixilho.Largura,
                Altura = caixilho.Altura,
                Quantidade = caixilho.Quantidade,
                PesoUnitario = caixilho.PesoUnitario,
                Liberado = caixilho.Liberado,
                DataLiberacao = caixilho.DataLiberacao,
                Observacoes = caixilho.Observacoes,
                DescricaoCaixilho = caixilho.DescricaoCaixilho,
                StatusProducao = MapStatusProducaoToInt(caixilho.StatusProducao),
                ObraId = caixilho.ObraId,
                IdFamiliaCaixilho = caixilho.IdFamiliaCaixilho
            };
            await PutAsync($"/api/caixilho/{caixilho.IdCaixilho}", dto);
        }

        public async Task DeleteAsync(int id)
        {
            await base.DeleteAsync($"/api/caixilho/{id}");
        }

        public async Task LiberarAsync(int id)
        {
            await PostAsync($"/api/caixilho/{id}/liberar");
        }

        public async Task<List<Caixilho>> GetByFamiliaIdAsync(int familiaId)
        {
            var dtos = await GetListAsync<CaixilhoResponseDto>("/api/caixilho");
            return dtos.Where(c => c.IdFamiliaCaixilho == familiaId).Select(MapToModel).ToList();
        }

        private static Caixilho MapToModel(CaixilhoResponseDto dto)
        {
            return new Caixilho
            {
                IdCaixilho = dto.IdCaixilho,
                NomeCaixilho = dto.NomeCaixilho,
                Largura = dto.Largura,
                Altura = dto.Altura,
                Quantidade = dto.Quantidade,
                PesoUnitario = dto.PesoUnitario,
                Liberado = dto.Liberado,
                DataLiberacao = dto.DataLiberacao,
                Observacoes = dto.Observacoes,
                DescricaoCaixilho = dto.DescricaoCaixilho,
                StatusProducao = MapStatusProducao(dto.StatusProducao),
                ObraId = dto.ObraId,
                IdFamiliaCaixilho = dto.IdFamiliaCaixilho,
                Obra = dto.NomeObra != null ? new Obra { IdObra = dto.ObraId, Nome = dto.NomeObra } : null,
                FamiliaCaixilho = dto.DescricaoFamilia != null ? new FamiliaCaixilho { IdFamiliaCaixilho = dto.IdFamiliaCaixilho, DescricaoFamilia = dto.DescricaoFamilia } : null
            };
        }

        private static string MapStatusProducao(int status)
        {
            return status switch
            {
                0 => "Pendente",     // default (DB default = 0)
                1 => "ParaMedir",    // ParaMedir = 1
                2 => "Medido",       // Medido = 2
                3 => "Concluido",    // Concluido = 3
                4 => "Pendente",     // Pendente = 4
                _ => "Pendente"
            };
        }

        private static int MapStatusProducaoToInt(string status)
        {
            return status switch
            {
                "ParaMedir" => 1,
                "Medido" => 2,
                "Concluido" => 3,
                "Pendente" => 0,
                _ => 0
            };
        }
    }
}
