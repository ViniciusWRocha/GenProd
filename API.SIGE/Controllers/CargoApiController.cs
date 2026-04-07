using API.SIGE.DTOs;
using API.SIGE.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.SIGE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CargoApiController : ControllerBase
{
    private readonly ICargoService _cargoService;

    public CargoApiController(ICargoService cargoService)
    {
        _cargoService = cargoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CargoResponseDto>>> GetAll()
    {
        var lista = await _cargoService.GetAllAsync();
        return Ok(lista);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CargoResponseDto>> GetById(int id)
    {
        var item = await _cargoService.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<CargoResponseDto>> Create([FromBody] CargoCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var cargo = await _cargoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = cargo.IdCargo }, cargo);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] CargoCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var existente = await _cargoService.GetByIdAsync(id);
        if (existente == null) return NotFound();

        await _cargoService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existente = await _cargoService.GetByIdAsync(id);
        if (existente == null) return NotFound();

        await _cargoService.DeleteAsync(id);
        return NoContent();
    }
}
