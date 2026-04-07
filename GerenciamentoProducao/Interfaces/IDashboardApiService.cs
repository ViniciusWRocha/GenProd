using GerenciamentoProducao.ApiDtos;

namespace GerenciamentoProducao.Interfaces
{
    public interface IDashboardApiService
    {
        Task<DashboardMetricasDto> GetMetricasAsync();
        Task<List<ObraResponseDto>> GetObrasEmAndamentoAsync();
        Task<List<ObraResponseDto>> GetObrasRecentesAsync(int quantidade = 5);
    }
}
