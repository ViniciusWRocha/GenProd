using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using GerenciamentoProducao.ApiDtos;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoProducao.Controllers.Api;

/// <summary>
/// Endpoint para o app mobile enviar foto da medição (família de caixilhos), usando o mesmo JWT da API SIGE.
/// </summary>
[AllowAnonymous]
[ApiController]
[Route("api/familia-caixilho")]
public class FamiliaCaixilhoMedicaoApiController : ControllerBase
{
    private const int MaxFotoBase64Length = 6_500_000;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IFamiliaMedicaoFotoStore _medicaoFotoStore;

    public FamiliaCaixilhoMedicaoApiController(
        IHttpClientFactory httpClientFactory,
        IFamiliaMedicaoFotoStore medicaoFotoStore)
    {
        _httpClientFactory = httpClientFactory;
        _medicaoFotoStore = medicaoFotoStore;
    }

    [HttpPost("{id:int}/medicao-foto")]
    public async Task<IActionResult> PostMedicaoFoto(int id, [FromBody] MedicaoFotoUploadDto dto, CancellationToken cancellationToken)
    {
        var auth = Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(auth) || !auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();

        if (dto?.FotoBase64 is not { Length: > 0 } foto)
            return BadRequest(new { message = "FotoBase64 é obrigatório." });

        foto = foto.Trim();
        if (foto.Length > MaxFotoBase64Length)
            return BadRequest(new { message = "Imagem demasiado grande." });

        if (!foto.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
            foto = "data:image/jpeg;base64," + foto;

        var apiClient = _httpClientFactory.CreateClient("ApiSige");
        var familiaResp = await SendAuthorizedAsync(apiClient, HttpMethod.Get, $"/api/familia-caixilho/{id}", auth, cancellationToken);
        if (familiaResp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            return Unauthorized();
        if (!familiaResp.IsSuccessStatusCode)
            return NotFound(new { message = "Família não encontrada ou sem permissão." });

        var familiaJson = await familiaResp.Content.ReadAsStringAsync(cancellationToken);
        var familia = JsonSerializer.Deserialize<FamiliaApiSnapshot>(familiaJson, JsonOpts);
        if (familia == null)
            return BadRequest();

        if (familia.StatusFamilia == 4 || familia.StatusFamilia == 5)
            return BadRequest(new { message = "Família já em produção ou produzida." });

        var caixResp = await SendAuthorizedAsync(apiClient, HttpMethod.Get, "/api/caixilho", auth, cancellationToken);
        if (!caixResp.IsSuccessStatusCode)
            return StatusCode(502, new { message = "Não foi possível validar os caixilhos na API." });

        var caixilhos = await caixResp.Content.ReadFromJsonAsync<List<CaixilhoApiSnapshot>>(JsonOpts, cancellationToken)
                        ?? new List<CaixilhoApiSnapshot>();
        var daFamilia = caixilhos.Where(c => c.IdFamiliaCaixilho == id).ToList();
        if (daFamilia.Count == 0)
            return BadRequest(new { message = "Não há caixilhos nesta família." });
        if (daFamilia.All(c => c.StatusProducao == 2))
            return BadRequest(new { message = "Medição já confirmada (todos os caixilhos já estão medidos)." });

        if (await _medicaoFotoStore.GetAsync(id, cancellationToken) != null)
            return BadRequest(new { message = "Já existe uma foto aguardando aprovação na web." });

        await _medicaoFotoStore.SaveAsync(new FamiliaMedicaoFotoState
        {
            IdFamiliaCaixilho = id,
            FotoBase64 = foto,
            EnviadoEm = DateTime.UtcNow,
            EnviadoPor = "App mobile"
        });

        return Ok(new { message = "Foto recebida. Aguarde aprovação na web para marcar os caixilhos como medidos." });
    }

    private static Task<HttpResponseMessage> SendAuthorizedAsync(
        HttpClient client,
        HttpMethod method,
        string relativeUrl,
        string authorizationHeader,
        CancellationToken cancellationToken)
    {
        var req = new HttpRequestMessage(method, relativeUrl);
        req.Headers.Authorization = AuthenticationHeaderValue.Parse(authorizationHeader);
        return client.SendAsync(req, cancellationToken);
    }

    private sealed class FamiliaApiSnapshot
    {
        public int StatusFamilia { get; set; }
    }

    private sealed class CaixilhoApiSnapshot
    {
        public int IdFamiliaCaixilho { get; set; }
        public int StatusProducao { get; set; }
    }
}
