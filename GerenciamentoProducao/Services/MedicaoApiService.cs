using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Services;

public class MedicaoApiService : ApiClientBase
{
    public MedicaoApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        : base(httpClientFactory, httpContextAccessor) { }

    public async Task<MedicaoResponseDto?> GetByFamiliaAsync(int familiaId)
    {
        try
        {
            return await GetAsync<MedicaoResponseDto>($"api/medicao/familia/{familiaId}");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<List<AnexoResponseDto>> GetAnexosByMedicaoAsync(int medicaoId)
    {
        try
        {
            return await GetListAsync<AnexoResponseDto>($"api/anexo/medicao/{medicaoId}");
        }
        catch (HttpRequestException)
        {
            return new List<AnexoResponseDto>();
        }
    }

    public async Task<byte[]?> DownloadAnexoAsync(int anexoId)
    {
        try
        {
            return await GetBytesAsync($"api/anexo/{anexoId}/download");
        }
        catch
        {
            return null;
        }
    }

    public async Task<ProducaoFamiliaResponseDto?> GetProducaoByFamiliaAsync(int familiaId)
    {
        try
        {
            return await GetAsync<ProducaoFamiliaResponseDto>($"api/producao-familia/familia/{familiaId}");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
}
