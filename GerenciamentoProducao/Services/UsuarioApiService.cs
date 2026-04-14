using GerenciamentoProducao.ApiDtos;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Services
{
    public class UsuarioApiService : ApiClientBase, IUsuarioRepository
    {
        public UsuarioApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor) { }

        public async Task<List<Usuario>> GetAllAsync()
        {
            var dtos = await GetListAsync<UsuarioResponseDto>("/api/usuario");
            return dtos.Select(MapToModel).ToList();
        }

        public async Task<List<Usuario>> GetAllAtivosAsync()
        {
            var dtos = await GetListAsync<UsuarioResponseDto>("/api/usuario?ativos=true");
            return dtos.Select(MapToModel).ToList();
        }

        public async Task<List<Usuario>> GetAllInativosAsync()
        {
            var dtos = await GetListAsync<UsuarioResponseDto>("/api/usuario/inativos");
            return dtos.Select(MapToModel).ToList();
        }

        public async Task<Usuario> GetById(int id)
        {
            var dto = await GetAsync<UsuarioResponseDto>($"/api/usuario/{id}");
            return dto != null ? MapToModel(dto) : throw new Exception("Usuário não encontrado");
        }

        public async Task AddAsync(Usuario usuario)
        {
            var dto = new UsuarioCreateDto
            {
                NomeUsuario = usuario.NomeUsuario,
                Email = usuario.Email,
                Senha = usuario.Senha,
                Telefone = LimparTelefone(usuario.Telefone),
                IdTipoUsuario = usuario.IdTipoUsuario
            };
            await PostAsync<UsuarioCreateDto>("/api/usuario", dto);
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            var dto = new UsuarioUpdateDto
            {
                NomeUsuario = usuario.NomeUsuario,
                Email = usuario.Email,
                Senha = !string.IsNullOrEmpty(usuario.Senha) ? usuario.Senha : null,
                Telefone = LimparTelefone(usuario.Telefone),
                IdTipoUsuario = usuario.IdTipoUsuario
            };
            await PutAsync($"/api/usuario/{usuario.IdUsuario}", dto);
        }

        public async Task Delete(int id)
        {
            await InativarAsync(id);
        }

        public async Task InativarAsync(int id)
        {
            await base.DeleteAsync($"/api/usuario/{id}");
        }

        public async Task ReativarAsync(int id)
        {
            await PostAsync($"/api/usuario/{id}/ativar");
        }

        public async Task<(Usuario? Usuario, string? Token)> ValidarLoginAsync(string email, string senha)
        {
            var loginDto = new LoginRequestDto { Email = email, Senha = senha };
            var response = await PostAsync<LoginRequestDto, LoginResponseDto>("/api/usuario/login", loginDto);

            if (response == null || !response.Success)
                return (null, null);

            var usuario = new Usuario
            {
                IdUsuario = response.IdUsuario ?? 0,
                NomeUsuario = response.NomeUsuario ?? string.Empty,
                Email = response.Email ?? string.Empty,
                Ativo = true,
                IdTipoUsuario = response.TipoUsuario ?? 0,
                TipoUsuario = new TipoUsuario
                {
                    IdTipoUsuario = response.TipoUsuario ?? 0,
                    NomeTipoUsuario = response.NomeTipoUsuario ?? response.Cargo ?? "Funcionario"
                }
            };

            return (usuario, response.Token);
        }

        private static string? LimparTelefone(string? telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone)) return null;
            return new string(telefone.Where(char.IsDigit).ToArray());
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
