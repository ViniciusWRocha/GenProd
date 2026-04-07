using GerenciamentoProducao.ApiDtos;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Services
{
    public class FamiliaCaixilhoApiService : ApiClientBase, IFamiliaCaixilhoRepository
    {
        public FamiliaCaixilhoApiService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<List<FamiliaCaixilho>> GetAllAsync()
        {
            var dtos = await GetListAsync<FamiliaCaixilhoResponseDto>("/api/FamiliaCaixilhoApi");
            return dtos.Select(MapToModel).ToList();
        }

        public async Task<FamiliaCaixilho> GetByIdAsync(int id)
        {
            var dto = await GetAsync<FamiliaCaixilhoResponseDto>($"/api/FamiliaCaixilhoApi/{id}");
            return dto != null ? MapToModel(dto) : throw new Exception("Família não encontrada");
        }

        public async Task AddAsync(FamiliaCaixilho familiaCaixilho)
        {
            var dto = new FamiliaCaixilhoCreateDto
            {
                DescricaoFamilia = familiaCaixilho.DescricaoFamilia,
                IdObra = familiaCaixilho.IdObra
            };
            await PostAsync<FamiliaCaixilhoCreateDto>("/api/FamiliaCaixilhoApi", dto);
        }

        public async Task UpdateAsync(FamiliaCaixilho familiaCaixilho)
        {
            var dto = new FamiliaCaixilhoUpdateDto
            {
                DescricaoFamilia = familiaCaixilho.DescricaoFamilia,
                StatusFamilia = MapStatusFamiliaToInt(familiaCaixilho.StatusFamilia)
            };
            await PutAsync($"/api/FamiliaCaixilhoApi/{familiaCaixilho.IdFamiliaCaixilho}", dto);
        }

        public async Task DeleteAsync(int id)
        {
            await base.DeleteAsync($"/api/FamiliaCaixilhoApi/{id}");
        }

        public async Task<float> CalcularPesoTotalAsync(int familiaId)
        {
            var dto = await GetAsync<FamiliaCaixilhoResponseDto>($"/api/FamiliaCaixilhoApi/{familiaId}");
            return dto?.PesoTotal ?? 0;
        }

        public async Task AtualizarPesoTotalAsync(int familiaId)
        {
            await PostAsync("/api/FamiliaCaixilhoApi/recalcular-pesos");
        }

        public async Task<List<FamiliaCaixilho>> GetByObraIdAsync(int obraId)
        {
            var dtos = await GetListAsync<FamiliaCaixilhoResponseDto>($"/api/FamiliaCaixilhoApi/obra/{obraId}");
            return dtos.Select(MapToModel).ToList();
        }

        private static int MapStatusFamiliaToInt(string status)
        {
            return status switch
            {
                "Pendente" => 1,
                "EmMedicao" => 2,
                "Medida" => 3,
                "EmProducao" => 4,
                "Produzida" => 5,
                _ => 1
            };
        }

        private static FamiliaCaixilho MapToModel(FamiliaCaixilhoResponseDto dto)
        {
            return new FamiliaCaixilho
            {
                IdFamiliaCaixilho = dto.IdFamiliaCaixilho,
                DescricaoFamilia = dto.DescricaoFamilia,
                PesoTotal = dto.PesoTotal,
                IdObra = dto.IdObra,
                StatusFamilia = dto.StatusFamilia,
                QuantidadeCaixilhos = dto.QuantidadeCaixilhos
            };
        }
    }
}
