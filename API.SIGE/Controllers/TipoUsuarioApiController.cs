using API.SIGE.DTOs;
using API.SIGE.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.SIGE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipoUsuarioApiController : ControllerBase
{
    private readonly ITipoUsuarioService _tipoUsuarioService;

    public TipoUsuarioApiController(ITipoUsuarioService tipoUsuarioService)
    {
        _tipoUsuarioService = tipoUsuarioService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TipoUsuarioResponseDto>>> GetAll()
    {
        var lista = await _tipoUsuarioService.GetAllAsync();
        return Ok(lista);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TipoUsuarioResponseDto>> GetById(int id)
    {
        var item = await _tipoUsuarioService.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<TipoUsuarioResponseDto>> Create([FromBody] TipoUsuarioCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var tipo = await _tipoUsuarioService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = tipo.IdTipoUsuario }, tipo);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] TipoUsuarioCreateDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var existente = await _tipoUsuarioService.GetByIdAsync(id);
        if (existente == null) return NotFound();

        await _tipoUsuarioService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existente = await _tipoUsuarioService.GetByIdAsync(id);
        if (existente == null) return NotFound();

        await _tipoUsuarioService.DeleteAsync(id);
        return NoContent();
    }
}
