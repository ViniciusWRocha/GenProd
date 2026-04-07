using API.SIGE.DTOs;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Interfaces.Services;
using API.SIGE.Models;

namespace API.SIGE.Services
{
    public class TipoUsuarioService : ITipoUsuarioService
    {
        private readonly ITipoUsuarioRepository _tipoUsuarioRepository;

        public TipoUsuarioService(ITipoUsuarioRepository tipoUsuarioRepository)
        {
            _tipoUsuarioRepository = tipoUsuarioRepository;
        }

        public async Task<List<TipoUsuarioResponseDto>> GetAllAsync()
        {
            var tipos = await _tipoUsuarioRepository.GetAllAsync();
            return tipos.Select(MapToDto).ToList();
        }

        public async Task<TipoUsuarioResponseDto?> GetByIdAsync(int id)
        {
            var tipo = await _tipoUsuarioRepository.GetById(id);
            return tipo == null ? null : MapToDto(tipo);
        }

        public async Task<TipoUsuarioResponseDto> CreateAsync(TipoUsuarioCreateDto dto)
        {
            var tipo = new TipoUsuario
            {
                NomeTipoUsuario = dto.NomeTipoUsuario
            };

            await _tipoUsuarioRepository.AddAsync(tipo);
            return MapToDto(tipo);
        }

        public async Task UpdateAsync(int id, TipoUsuarioCreateDto dto)
        {
            var tipo = await _tipoUsuarioRepository.GetById(id);
            if (tipo == null)
                throw new InvalidOperationException($"TipoUsuario com ID {id} não encontrado.");

            tipo.NomeTipoUsuario = dto.NomeTipoUsuario;
            await _tipoUsuarioRepository.UpdateAsync(tipo);
        }

        public async Task DeleteAsync(int id)
        {
            await _tipoUsuarioRepository.DeleteAsync(id);
        }

        private static TipoUsuarioResponseDto MapToDto(TipoUsuario tipo)
        {
            return new TipoUsuarioResponseDto
            {
                IdTipoUsuario = tipo.IdTipoUsuario,
                NomeTipoUsuario = tipo.NomeTipoUsuario
            };
        }
    }
}
