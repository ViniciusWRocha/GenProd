using API.SIGE.DTOs;

namespace API.SIGE.Interfaces.Services
{
    public interface IAnexoService
    {
        Task<AnexoResponseDto> UploadAsync(AnexoUploadDto dto);
        Task<List<AnexoResponseDto>> GetByMedicaoIdAsync(int medicaoId);
        Task<List<AnexoResponseDto>> GetByProducaoFamiliaIdAsync(int producaoFamiliaId);
        Task DeleteAsync(int id);
    }
}
