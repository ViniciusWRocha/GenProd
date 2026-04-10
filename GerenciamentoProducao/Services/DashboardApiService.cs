using GerenciamentoProducao.ApiDtos;
using GerenciamentoProducao.Interfaces;

namespace GerenciamentoProducao.Services
{
    public class DashboardApiService : ApiClientBase, IDashboardApiService
    {
        public DashboardApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor) { }

        public async Task<DashboardMetricasDto> GetMetricasAsync()
        {
            var metricas = await GetAsync<DashboardMetricasDto>("/api/dashboard/metricas");
            return metricas ?? new DashboardMetricasDto();
        }

        public async Task<List<ObraResponseDto>> GetObrasEmAndamentoAsync()
        {
            return await GetListAsync<ObraResponseDto>("/api/obra?finalizadas=false");
        }

        public async Task<List<ObraResponseDto>> GetObrasRecentesAsync(int quantidade = 5)
        {
            var obras = await GetListAsync<ObraResponseDto>("/api/obra");
            return obras.OrderByDescending(o => o.DataInicio).Take(quantidade).ToList();
        }
    }
}
