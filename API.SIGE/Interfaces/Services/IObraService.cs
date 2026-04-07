using API.SIGE.DTOs;

namespace API.SIGE.Interfaces.Services
{
    public interface IObraService
    {
        Task<List<ObraResponseDto>> GetAllAsync(bool? finalizadas);
        Task<ObraResponseDto?> GetByIdAsync(int id);
        Task<ObraResponseDto> CreateAsync(ObraCreateDto dto);
        Task UpdateAsync(int id, ObraUpdateDto dto);
        Task DeleteAsync(int id);
        Task<List<string>> ImportarObrasXmlAsync(List<IFormFile> arquivosXml);
        Task VerificarAsync(int id);
        Task ConcluirAsync(int id);
        Task RecalcularProgressoAsync(int id);
    }
}
