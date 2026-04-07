using API.SIGE.DTOs;
using API.SIGE.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.SIGE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FamiliaCaixilhoApiController : ControllerBase
{
    private readonly IFamiliaCaixilhoService _familiaService;

    public FamiliaCaixilhoApiController(IFamiliaCaixilhoService familiaService)
    {
        _familiaService = familiaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<FamiliaCaixilhoResponseDto>>> GetAll()
    {
        var lista = await _familiaService.GetAllAsync();
        return Ok(lista);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FamiliaCaixilhoResponseDto>> GetById(int id)
    {
        var item = await _familiaService.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<FamiliaCaixilhoResponseDto>> Create([FromBody] FamiliaCaixilhoCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var familia = await _familiaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = familia.IdFamiliaCaixilho }, familia);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] FamiliaCaixilhoUpdateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var existente = await _familiaService.GetByIdAsync(id);
        if (existente == null) return NotFound();

        await _familiaService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existente = await _familiaService.GetByIdAsync(id);
        if (existente == null) return NotFound();

        await _familiaService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("obra/{obraId:int}")]
    public async Task<ActionResult<List<FamiliaCaixilhoResponseDto>>> GetByObraId(int obraId)
    {
        var familias = await _familiaService.GetByObraIdAsync(obraId);
        return Ok(familias);
    }

    [HttpPost("recalcular-pesos")]
    public async Task<ActionResult> RecalcularPesos()
    {
        var total = await _familiaService.RecalcularPesosAsync();
        return Ok(new { success = true, total });
    }
}
