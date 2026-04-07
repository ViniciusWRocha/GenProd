using API.SIGE.DTOs;

namespace API.SIGE.Interfaces.Services
{
    public interface ITipoUsuarioService
    {
        Task<List<TipoUsuarioResponseDto>> GetAllAsync();
        Task<TipoUsuarioResponseDto?> GetByIdAsync(int id);
        Task<TipoUsuarioResponseDto> CreateAsync(TipoUsuarioCreateDto dto);
        Task UpdateAsync(int id, TipoUsuarioCreateDto dto);
        Task DeleteAsync(int id);
    }
}
