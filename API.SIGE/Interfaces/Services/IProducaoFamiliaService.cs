using API.SIGE.DTOs;

namespace API.SIGE.Interfaces.Services
{
    public interface IProducaoFamiliaService
    {
        Task<ProducaoFamiliaResponseDto?> GetByIdAsync(int id);
        Task<ProducaoFamiliaResponseDto?> GetByFamiliaIdAsync(int familiaId);
        Task<ProducaoFamiliaResponseDto> IniciarAsync(int familiaId, ProducaoFamiliaIniciarDto dto);
        Task PausarAsync(int familiaId, ProducaoFamiliaPausarDto dto);
        Task<ProducaoFamiliaResponseDto> FinalizarAsync(int familiaId, ProducaoFamiliaFinalizarDto dto);
    }
}
