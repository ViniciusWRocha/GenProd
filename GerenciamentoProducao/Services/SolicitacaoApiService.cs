using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Services;

public class SolicitacaoApiService : ApiClientBase
{
    public SolicitacaoApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        : base(httpClientFactory, httpContextAccessor) { }

    public async Task<List<SolicitacaoClienteResponseDto>> GetByFamiliaAsync(int familiaId)
    {
        try
        {
            return await GetListAsync<SolicitacaoClienteResponseDto>($"api/solicitacao/familia/{familiaId}");
        }
        catch (HttpRequestException)
        {
            return new List<SolicitacaoClienteResponseDto>();
        }
    }
}