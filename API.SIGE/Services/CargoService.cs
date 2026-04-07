using API.SIGE.DTOs;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Interfaces.Services;
using API.SIGE.Models;

namespace API.SIGE.Services
{
    public class CargoService : ICargoService
    {
        private readonly ICargoRepository _cargoRepository;

        public CargoService(ICargoRepository cargoRepository)
        {
            _cargoRepository = cargoRepository;
        }

        public async Task<List<CargoResponseDto>> GetAllAsync()
        {
            var cargos = await _cargoRepository.GetAllAsync();
            return cargos.Select(MapToDto).ToList();
        }

        public async Task<CargoResponseDto?> GetByIdAsync(int id)
        {
            var cargo = await _cargoRepository.GetByIdAsync(id);
            return cargo == null ? null : MapToDto(cargo);
        }

        public async Task<CargoResponseDto> CreateAsync(CargoCreateDto dto)
        {
            var cargo = new Cargo
            {
                TipoCargo = (TipoCargo)dto.TipoCargo,
                DescricaoCargo = dto.DescricaoCargo
            };

            await _cargoRepository.AddAsync(cargo);
            return MapToDto(cargo);
        }

        public async Task UpdateAsync(int id, CargoCreateDto dto)
        {
            var cargo = await _cargoRepository.GetByIdAsync(id);
            if (cargo == null)
                throw new InvalidOperationException($"Cargo com ID {id} não encontrado.");

            cargo.TipoCargo = (TipoCargo)dto.TipoCargo;
            cargo.DescricaoCargo = dto.DescricaoCargo;
            await _cargoRepository.UpdateAsync(cargo);
        }

        public async Task DeleteAsync(int id)
        {
            await _cargoRepository.DeleteAsync(id);
        }

        private static CargoResponseDto MapToDto(Cargo cargo)
        {
            return new CargoResponseDto
            {
                IdCargo = cargo.IdCargo,
                TipoCargo = cargo.TipoCargo.ToString(),
                DescricaoCargo = cargo.DescricaoCargo
            };
        }
    }
}
