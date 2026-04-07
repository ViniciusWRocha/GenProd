using API.SIGE.DTOs;
using API.SIGE.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.SIGE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CaixilhoApiController : ControllerBase
{
    private readonly ICaixilhoService _caixilhoService;

    public CaixilhoApiController(ICaixilhoService caixilhoService)
    {
        _caixilhoService = caixilhoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CaixilhoResponseDto>>> GetAll()
    {
        var lista = await _caixilhoService.GetAllAsync();
        return Ok(lista);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CaixilhoResponseDto>> GetById(int id)
    {
        var item = await _caixilhoService.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<CaixilhoResponseDto>> Create([FromBody] CaixilhoCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var caixilho = await _caixilhoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = caixilho.IdCaixilho }, caixilho);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] CaixilhoUpdateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var existente = await _caixilhoService.GetByIdAsync(id);
        if (existente == null) return NotFound();

        await _caixilhoService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existente = await _caixilhoService.GetByIdAsync(id);
        if (existente == null) return NotFound();

        await _caixilhoService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("familia/{familiaId:int}")]
    public async Task<ActionResult<List<CaixilhoResponseDto>>> GetByFamiliaId(int familiaId)
    {
        var caixilhos = await _caixilhoService.GetByFamiliaIdAsync(familiaId);
        return Ok(caixilhos);
    }

    [HttpPost("{id:int}/liberar")]
    public async Task<ActionResult> Liberar(int id)
    {
        var existente = await _caixilhoService.GetByIdAsync(id);
        if (existente == null) return NotFound();

        await _caixilhoService.LiberarAsync(id);
        return Ok(new { success = true });
    }
}
