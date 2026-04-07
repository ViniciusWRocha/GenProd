using API.SIGE.DTOs;

namespace API.SIGE.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<DashboardMetricasDto> GetMetricasAsync();
        Task AtualizarProgressoObrasAsync();
    }
}
