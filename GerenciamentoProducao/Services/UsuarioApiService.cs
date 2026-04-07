using GerenciamentoProducao.ApiDtos;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Services
{
    public class UsuarioApiService : ApiClientBase, IUsuarioRepository
    {
        public UsuarioApiService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<List<Usuario>> GetAllAsync()
        {
            var dtos = await GetListAsync<UsuarioResponseDto>("/api/UsuarioApi");
            return dtos.Select(MapToModel).ToList();
        }

        public async Task<List<Usuario>> GetAllAtivosAsync()
        {
            var dtos = await GetListAsync<UsuarioResponseDto>("/api/UsuarioApi?ativos=true");
            return dtos.Select(MapToModel).ToList();
        }

        public async Task<List<Usuario>> GetAllInativosAsync()
        {
            var dtos = await GetListAsync<UsuarioResponseDto>("/api/UsuarioApi/inativos");
            return dtos.Select(MapToModel).ToList();
        }

        public async Task<Usuario> GetById(int id)
        {
            var dto = await GetAsync<UsuarioResponseDto>($"/api/UsuarioApi/{id}");
            return dto != null ? MapToModel(dto) : throw new Exception("Usuário não encontrado");
        }

        public async Task AddAsync(Usuario usuario)
        {
            var dto = new UsuarioCreateDto
            {
                NomeUsuario = usuario.NomeUsuario,
                Email = usuario.Email,
                Senha = usuario.Senha,
                Telefone = usuario.Telefone,
                IdTipoUsuario = usuario.IdTipoUsuario
            };
            await PostAsync<UsuarioCreateDto>("/api/UsuarioApi", dto);
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            var dto = new UsuarioUpdateDto
            {
                NomeUsuario = usuario.NomeUsuario,
                Email = usuario.Email,
                Senha = !string.IsNullOrEmpty(usuario.Senha) ? usuario.Senha : null,
                Telefone = usuario.Telefone,
                IdTipoUsuario = usuario.IdTipoUsuario
            };
            await PutAsync($"/api/UsuarioApi/{usuario.IdUsuario}", dto);
        }

        public async Task Delete(int id)
        {
            await InativarAsync(id);
        }

        public async Task InativarAsync(int id)
        {
            await base.DeleteAsync($"/api/UsuarioApi/{id}");
        }

        public async Task ReativarAsync(int id)
        {
            await PostAsync($"/api/UsuarioApi/{id}/ativar");
        }

        public async Task<Usuario>? ValidarLoginAsync(string email, string senha)
        {
            var loginDto = new LoginRequestDto { Email = email, Senha = senha };
            var response = await PostAsync<LoginRequestDto, LoginResponseDto>("/api/UsuarioApi/login", loginDto);

            if (response == null || !response.Success)
                return null!;

            return new Usuario
            {
                IdUsuario = response.IdUsuario ?? 0,
                NomeUsuario = response.NomeUsuario ?? string.Empty,
                Email = response.Email ?? string.Empty,
                Ativo = true,
                IdTipoUsuario = response.TipoUsuario ?? 0,
                TipoUsuario = new TipoUsuario
                {
                    IdTipoUsuario = response.TipoUsuario ?? 0,
                    NomeTipoUsuario = response.NomeTipoUsuario ?? "Funcionario"
                }
            };
        }

        private static Usuario MapToModel(UsuarioResponseDto dto)
        {
            return new Usuario
            {
                IdUsuario = dto.IdUsuario,
                NomeUsuario = dto.NomeUsuario,
                Email = dto.Email,
                Telefone = dto.Telefone,
                Ativo = dto.Ativo,
                IdTipoUsuario = dto.IdTipoUsuario,
                TipoUsuario = new TipoUsuario
                {
                    IdTipoUsuario = dto.IdTipoUsuario,
                    NomeTipoUsuario = dto.NomeTipoUsuario ?? string.Empty
                }
            };
        }
    }
}
