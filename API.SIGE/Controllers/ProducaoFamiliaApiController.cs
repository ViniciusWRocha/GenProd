using API.SIGE.DTOs;
using API.SIGE.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.SIGE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProducaoFamiliaApiController : ControllerBase
{
    private readonly IProducaoFamiliaService _producaoService;

    public ProducaoFamiliaApiController(IProducaoFamiliaService producaoService)
    {
        _producaoService = producaoService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProducaoFamiliaResponseDto>> GetById(int id)
    {
        var producao = await _producaoService.GetByIdAsync(id);
        if (producao == null) return NotFound();
        return Ok(producao);
    }

    [HttpGet("familia/{familiaId:int}")]
    public async Task<ActionResult<ProducaoFamiliaResponseDto>> GetByFamiliaId(int familiaId)
    {
        var producao = await _producaoService.GetByFamiliaIdAsync(familiaId);
        if (producao == null) return NotFound();
        return Ok(producao);
    }

    [HttpPost("{familiaId:int}/iniciar")]
    public async Task<ActionResult<ProducaoFamiliaResponseDto>> Iniciar(int familiaId, [FromBody] ProducaoFamiliaIniciarDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        try
        {
            var producao = await _producaoService.IniciarAsync(familiaId, dto);
            return Ok(producao);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{familiaId:int}/pausar")]
    public async Task<ActionResult> Pausar(int familiaId, [FromBody] ProducaoFamiliaPausarDto dto)
    {
        try
        {
            await _producaoService.PausarAsync(familiaId, dto);
            return Ok(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{familiaId:int}/finalizar")]
    public async Task<ActionResult<ProducaoFamiliaResponseDto>> Finalizar(int familiaId, [FromBody] ProducaoFamiliaFinalizarDto dto)
    {
        try
        {
            var producao = await _producaoService.FinalizarAsync(familiaId, dto);
            return Ok(producao);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
