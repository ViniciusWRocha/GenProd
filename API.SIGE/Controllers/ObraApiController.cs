using API.SIGE.DTOs;
using API.SIGE.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.SIGE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ObraApiController : ControllerBase
{
    private readonly IObraService _obraService;

    public ObraApiController(IObraService obraService)
    {
        _obraService = obraService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ObraResponseDto>>> GetAll([FromQuery] bool? finalizadas = null)
    {
        var obras = await _obraService.GetAllAsync(finalizadas);
        return Ok(obras);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ObraResponseDto>> GetById(int id)
    {
        var obra = await _obraService.GetByIdAsync(id);
        if (obra == null) return NotFound();
        return Ok(obra);
    }

    [HttpPost]
    public async Task<ActionResult<ObraResponseDto>> Create([FromBody] ObraCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var obra = await _obraService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = obra.IdObra }, obra);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] ObraUpdateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var existente = await _obraService.GetByIdAsync(id);
        if (existente == null) return NotFound();

        await _obraService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existente = await _obraService.GetByIdAsync(id);
        if (existente == null) return NotFound();

        await _obraService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:int}/verificar")]
    public async Task<ActionResult> Verificar(int id)
    {
        try
        {
            await _obraService.VerificarAsync(id);
            return Ok(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{id:int}/concluir")]
    public async Task<ActionResult> Concluir(int id)
    {
        try
        {
            await _obraService.ConcluirAsync(id);
            return Ok(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{id:int}/recalcular-progresso")]
    public async Task<ActionResult> RecalcularProgresso(int id)
    {
        try
        {
            await _obraService.RecalcularProgressoAsync(id);
            return Ok(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
