using API.SIGE.DTOs;
using API.SIGE.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.SIGE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnexoApiController : ControllerBase
{
    private readonly IAnexoService _anexoService;

    public AnexoApiController(IAnexoService anexoService)
    {
        _anexoService = anexoService;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<AnexoResponseDto>> Upload([FromForm] AnexoUploadDto dto)
    {
        try
        {
            var anexo = await _anexoService.UploadAsync(dto);
            return Ok(anexo);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("medicao/{medicaoId:int}")]
    public async Task<ActionResult<List<AnexoResponseDto>>> GetByMedicaoId(int medicaoId)
    {
        var anexos = await _anexoService.GetByMedicaoIdAsync(medicaoId);
        return Ok(anexos);
    }

    [HttpGet("producao/{producaoId:int}")]
    public async Task<ActionResult<List<AnexoResponseDto>>> GetByProducaoFamiliaId(int producaoId)
    {
        var anexos = await _anexoService.GetByProducaoFamiliaIdAsync(producaoId);
        return Ok(anexos);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _anexoService.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }
}
