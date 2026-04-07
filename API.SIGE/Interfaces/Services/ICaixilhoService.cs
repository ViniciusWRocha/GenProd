using API.SIGE.DTOs;

namespace API.SIGE.Interfaces.Services
{
    public interface ICaixilhoService
    {
        Task<List<CaixilhoResponseDto>> GetAllAsync();
        Task<CaixilhoResponseDto?> GetByIdAsync(int id);
        Task<CaixilhoResponseDto> CreateAsync(CaixilhoCreateDto dto);
        Task UpdateAsync(int id, CaixilhoUpdateDto dto);
        Task DeleteAsync(int id);
        Task LiberarAsync(int id);
        Task<List<CaixilhoResponseDto>> GetByFamiliaIdAsync(int familiaId);
    }
}
