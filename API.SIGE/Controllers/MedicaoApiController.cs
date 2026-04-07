using API.SIGE.DTOs;
using API.SIGE.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.SIGE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicaoApiController : ControllerBase
{
    private readonly IMedicaoService _medicaoService;

    public MedicaoApiController(IMedicaoService medicaoService)
    {
        _medicaoService = medicaoService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MedicaoResponseDto>> GetById(int id)
    {
        var medicao = await _medicaoService.GetByIdAsync(id);
        if (medicao == null) return NotFound();
        return Ok(medicao);
    }

    [HttpGet("familia/{familiaId:int}")]
    public async Task<ActionResult<MedicaoResponseDto>> GetByFamiliaId(int familiaId)
    {
        var medicao = await _medicaoService.GetByFamiliaIdAsync(familiaId);
        if (medicao == null) return NotFound();
        return Ok(medicao);
    }

    [HttpPost("{familiaId:int}/iniciar")]
    public async Task<ActionResult<MedicaoResponseDto>> Iniciar(int familiaId, [FromBody] MedicaoIniciarDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        try
        {
            var medicao = await _medicaoService.IniciarAsync(familiaId, dto);
            return Ok(medicao);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{familiaId:int}/pausar")]
    public async Task<ActionResult> Pausar(int familiaId, [FromBody] MedicaoPausarDto dto)
    {
        try
        {
            await _medicaoService.PausarAsync(familiaId, dto);
            return Ok(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("{familiaId:int}/finalizar")]
    public async Task<ActionResult<MedicaoResponseDto>> Finalizar(int familiaId, [FromBody] MedicaoFinalizarDto dto)
    {
        try
        {
            var medicao = await _medicaoService.FinalizarAsync(familiaId, dto);
            return Ok(medicao);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
