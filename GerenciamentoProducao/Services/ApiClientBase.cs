using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GerenciamentoProducao.Services
{
    public abstract class ApiClientBase
    {
        protected readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        protected ApiClientBase(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("ApiSige");
            _httpContextAccessor = httpContextAccessor;
        }

        private void SetAuthHeader()
        {
            var token = _httpContextAccessor.HttpContext?.User?.FindFirst("ApiToken")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private static async Task EnsureSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"API retornou {(int)response.StatusCode} ({response.StatusCode}): {body}");
            }
        }

        protected async Task<T?> GetAsync<T>(string endpoint)
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync(endpoint);
            await EnsureSuccess(response);
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }

        protected async Task<List<T>> GetListAsync<T>(string endpoint)
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync(endpoint);
            await EnsureSuccess(response);
            return await response.Content.ReadFromJsonAsync<List<T>>(_jsonOptions) ?? new List<T>();
        }

        protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            SetAuthHeader();
            var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonOptions);
            await EnsureSuccess(response);
            return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
        }

        protected async Task PostAsync<TRequest>(string endpoint, TRequest data)
        {
            SetAuthHeader();
            var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonOptions);
            await EnsureSuccess(response);
        }

        protected async Task PostAsync(string endpoint)
        {
            SetAuthHeader();
            var response = await _httpClient.PostAsync(endpoint, null);
            await EnsureSuccess(response);
        }

        protected async Task PutAsync<TRequest>(string endpoint, TRequest data)
        {
            SetAuthHeader();
            var response = await _httpClient.PutAsJsonAsync(endpoint, data, _jsonOptions);
            await EnsureSuccess(response);
        }

        protected async Task<byte[]?> GetBytesAsync(string endpoint)
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsByteArrayAsync();
        }

        protected async Task DeleteAsync(string endpoint)
        {
            SetAuthHeader();
            var response = await _httpClient.DeleteAsync(endpoint);
            await EnsureSuccess(response);
        }

        protected async Task<HttpResponseMessage> PostMultipartAsync(string endpoint, MultipartFormDataContent content)
        {
            SetAuthHeader();
            var response = await _httpClient.PostAsync(endpoint, content);
            await EnsureSuccess(response);
            return response;
        }
    }
}
