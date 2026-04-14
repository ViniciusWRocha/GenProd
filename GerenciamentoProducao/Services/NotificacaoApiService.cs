namespace GerenciamentoProducao.Services;

public class NotificacaoApiService : ApiClientBase
{
    public NotificacaoApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        : base(httpClientFactory, httpContextAccessor) { }

    public async Task<List<NotificacaoWebDto>> GetByUsuarioAsync(int idUsuario)
    {
        try
        {
            return await GetListAsync<NotificacaoWebDto>($"api/notificacao/{idUsuario}");
        }
        catch (HttpRequestException)
        {
            return new List<NotificacaoWebDto>();
        }
    }

    public async Task<int> GetNaoLidasCountAsync(int idUsuario)
    {
        try
        {
            var result = await GetAsync<NaoLidasCountResult>($"api/notificacao/{idUsuario}/nao-lidas/count");
            return result?.Count ?? 0;
        }
        catch (HttpRequestException)
        {
            return 0;
        }
    }

    public async Task MarcarLidaAsync(int idNotificacao)
    {
        try
        {
            await PostAsync($"api/notificacao/{idNotificacao}/marcar-lida");
        }
        catch (HttpRequestException) { }
    }

    public async Task MarcarTodasLidasAsync(int idUsuario)
    {
        try
        {
            var lista = await GetByUsuarioAsync(idUsuario);
            var naoLidas = lista.Where(n => !n.Lida).ToList();
            if (naoLidas.Count == 0) return;

            await Task.WhenAll(naoLidas.Select(n => MarcarLidaAsync(n.IdNotificacao)));
        }
        catch (HttpRequestException) { }
    }

    public async Task ApagarAsync(int idNotificacao)
    {
        await DeleteAsync($"api/notificacao/{idNotificacao}");
    }

    public async Task ApagarLidasAsync(int idUsuario)
    {
        await DeleteAsync($"api/notificacao/usuario/{idUsuario}/lidas");
    }

    public async Task BroadcastAsync(string titulo, string mensagem, int tipoNotificacao, int? idObra, int tipoCargo)
    {
        try
        {
            await PostAsync<object>("api/notificacao/broadcast", new
            {
                titulo,
                mensagem,
                tipoNotificacao,
                idObra,
                tipoCargo
            });
        }
        catch (HttpRequestException) { }
    }

    public class NotificacaoWebDto
    {
        public int IdNotificacao { get; set; }
        public int IdUsuarioDestino { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public bool Lida { get; set; }
        public DateTime DataCriacao { get; set; }
        public int TipoNotificacao { get; set; }
        public int? IdObra { get; set; }
    }

    private class NaoLidasCountResult
    {
        public int Count { get; set; }
    }
}
