using API.SIGE.DTOs;

namespace API.SIGE.Interfaces.Services
{
    public interface ICargoService
    {
        Task<List<CargoResponseDto>> GetAllAsync();
        Task<CargoResponseDto?> GetByIdAsync(int id);
        Task<CargoResponseDto> CreateAsync(CargoCreateDto dto);
        Task UpdateAsync(int id, CargoCreateDto dto);
        Task DeleteAsync(int id);
    }
}
