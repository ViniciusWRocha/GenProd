using API.SIGE.DTOs;

namespace API.SIGE.Interfaces.Services
{
    public interface IFamiliaCaixilhoService
    {
        Task<List<FamiliaCaixilhoResponseDto>> GetAllAsync();
        Task<FamiliaCaixilhoResponseDto?> GetByIdAsync(int id);
        Task<FamiliaCaixilhoResponseDto> CreateAsync(FamiliaCaixilhoCreateDto dto);
        Task UpdateAsync(int id, FamiliaCaixilhoUpdateDto dto);
        Task DeleteAsync(int id);
        Task<int> RecalcularPesosAsync();
        Task<List<FamiliaCaixilhoResponseDto>> GetByObraIdAsync(int obraId);
    }
}
