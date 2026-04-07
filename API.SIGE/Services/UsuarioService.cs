using API.SIGE.DTOs;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Interfaces.Services;
using API.SIGE.Models;

namespace API.SIGE.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioCargoRepository _usuarioCargoRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository, IUsuarioCargoRepository usuarioCargoRepository)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioCargoRepository = usuarioCargoRepository;
        }

        public async Task<List<UsuarioResponseDto>> GetAllAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return usuarios.Select(MapToDto).ToList();
        }

        public async Task<List<UsuarioResponseDto>> GetAllAtivosAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAtivosAsync();
            return usuarios.Select(MapToDto).ToList();
        }

        public async Task<List<UsuarioResponseDto>> GetAllInativosAsync()
        {
            var usuarios = await _usuarioRepository.GetAllInativosAsync();
            return usuarios.Select(MapToDto).ToList();
        }

        public async Task<UsuarioResponseDto?> GetByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetById(id);
            return usuario == null ? null : MapToDto(usuario);
        }

        public async Task<UsuarioResponseDto> CreateAsync(UsuarioCreateDto dto)
        {
            var usuario = new Usuario
            {
                NomeUsuario = dto.NomeUsuario,
                Email = dto.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
                Telefone = dto.Telefone,
                IdTipoUsuario = dto.IdTipoUsuario,
                Ativo = true
            };

            await _usuarioRepository.AddAsync(usuario);

            // Atribuir cargos se informados
            if (dto.IdCargos != null)
            {
                foreach (var idCargo in dto.IdCargos)
                {
                    await _usuarioCargoRepository.AddAsync(new UsuarioCargo
                    {
                        IdUsuario = usuario.IdUsuario,
                        IdCargo = idCargo
                    });
                }
            }

            var result = await _usuarioRepository.GetById(usuario.IdUsuario);
            return MapToDto(result!);
        }

        public async Task UpdateAsync(int id, UsuarioUpdateDto dto)
        {
            var usuario = await _usuarioRepository.GetById(id);
            if (usuario == null)
                throw new InvalidOperationException($"Usuário com ID {id} não encontrado.");

            usuario.NomeUsuario = dto.NomeUsuario;
            usuario.Email = dto.Email;
            usuario.Telefone = dto.Telefone;
            usuario.IdTipoUsuario = dto.IdTipoUsuario;

            if (!string.IsNullOrWhiteSpace(dto.Senha))
            {
                usuario.Senha = BCrypt.Net.BCrypt.HashPassword(dto.Senha);
            }

            await _usuarioRepository.UpdateAsync(usuario);
        }

        public async Task InativarAsync(int id)
        {
            await _usuarioRepository.InativarAsync(id);
        }

        public async Task AtivarAsync(int id)
        {
            await _usuarioRepository.ReativarAsync(id);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);

            if (usuario == null || !usuario.Ativo)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Usuário ou senha inválidos."
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.Senha))
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Usuário ou senha inválidos."
                };
            }

            return new LoginResponseDto
            {
                Success = true,
                IdUsuario = usuario.IdUsuario,
                NomeUsuario = usuario.NomeUsuario,
                Email = usuario.Email,
                TipoUsuario = usuario.IdTipoUsuario,
                NomeTipoUsuario = usuario.TipoUsuario?.NomeTipoUsuario,
                Cargos = usuario.UsuarioCargos?
                    .Select(uc => uc.Cargo?.DescricaoCargo ?? "")
                    .Where(c => !string.IsNullOrEmpty(c))
                    .ToList()
            };
        }

        public async Task<List<UsuarioResponseDto>> GetByCargoAsync(int tipoCargo)
        {
            var usuarios = await _usuarioRepository.GetByCargoAsync((TipoCargo)tipoCargo);
            return usuarios.Select(MapToDto).ToList();
        }

        public async Task AtribuirCargoAsync(int idUsuario, int idCargo)
        {
            var exists = await _usuarioCargoRepository.ExistsAsync(idUsuario, idCargo);
            if (exists)
                throw new InvalidOperationException("Usuário já possui este cargo.");

            await _usuarioCargoRepository.AddAsync(new UsuarioCargo
            {
                IdUsuario = idUsuario,
                IdCargo = idCargo
            });
        }

        public async Task RemoverCargoAsync(int idUsuario, int idCargo)
        {
            await _usuarioCargoRepository.DeleteAsync(idUsuario, idCargo);
        }

        private static UsuarioResponseDto MapToDto(Usuario usuario)
        {
            return new UsuarioResponseDto
            {
                IdUsuario = usuario.IdUsuario,
                NomeUsuario = usuario.NomeUsuario,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Ativo = usuario.Ativo,
                IdTipoUsuario = usuario.IdTipoUsuario,
                NomeTipoUsuario = usuario.TipoUsuario?.NomeTipoUsuario,
                Cargos = usuario.UsuarioCargos?
                    .Where(uc => uc.Cargo != null)
                    .Select(uc => new CargoResponseDto
                    {
                        IdCargo = uc.Cargo!.IdCargo,
                        TipoCargo = uc.Cargo.TipoCargo.ToString(),
                        DescricaoCargo = uc.Cargo.DescricaoCargo
                    })
                    .ToList() ?? new List<CargoResponseDto>()
            };
        }
    }
}
