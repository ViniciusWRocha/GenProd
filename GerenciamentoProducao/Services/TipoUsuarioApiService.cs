using GerenciamentoProducao.ApiDtos;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Services
{
    public class TipoUsuarioApiService : ApiClientBase, ITipoUsuarioRepository
    {
        public TipoUsuarioApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor) { }

        public async Task<List<TipoUsuario>> GetAllAsync()
        {
            var dtos = await GetListAsync<TipoUsuarioResponseDto>("/api/tipo-usuario");
            return dtos.Select(d => new TipoUsuario
            {
                IdTipoUsuario = d.IdTipoUsuario,
                NomeTipoUsuario = d.NomeTipoUsuario
            }).ToList();
        }

        public async Task<TipoUsuario> GetById(int id)
        {
            var dto = await GetAsync<TipoUsuarioResponseDto>($"/api/tipo-usuario/{id}");
            if (dto == null) throw new Exception("Tipo de Usuário não encontrado");
            return new TipoUsuario
            {
                IdTipoUsuario = dto.IdTipoUsuario,
                NomeTipoUsuario = dto.NomeTipoUsuario
            };
        }

        public async Task AddAsync(TipoUsuario tipoUsuario)
        {
            var dto = new TipoUsuarioCreateDto { NomeTipoUsuario = tipoUsuario.NomeTipoUsuario };
            await PostAsync<TipoUsuarioCreateDto>("/api/tipo-usuario", dto);
        }

        public async Task UpdateAsync(TipoUsuario tipoUsuario)
        {
            var dto = new TipoUsuarioCreateDto { NomeTipoUsuario = tipoUsuario.NomeTipoUsuario };
            await PutAsync($"/api/tipo-usuario/{tipoUsuario.IdTipoUsuario}", dto);
        }

        public async Task DeleteAsync(int id)
        {
            await base.DeleteAsync($"/api/tipo-usuario/{id}");
        }
    }
}
