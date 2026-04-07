using API.SIGE.DTOs;
using API.SIGE.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.SIGE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacaoApiController : ControllerBase
{
    private readonly INotificacaoService _notificacaoService;

    public NotificacaoApiController(INotificacaoService notificacaoService)
    {
        _notificacaoService = notificacaoService;
    }

    [HttpGet("{idUsuario:int}")]
    public async Task<ActionResult<List<NotificacaoResponseDto>>> GetByUsuarioId(int idUsuario)
    {
        var notificacoes = await _notificacaoService.GetByUsuarioIdAsync(idUsuario);
        return Ok(notificacoes);
    }

    [HttpGet("{idUsuario:int}/nao-lidas")]
    public async Task<ActionResult<List<NotificacaoResponseDto>>> GetNaoLidas(int idUsuario)
    {
        var notificacoes = await _notificacaoService.GetNaoLidasAsync(idUsuario);
        return Ok(notificacoes);
    }

    [HttpPost("{id:int}/marcar-lida")]
    public async Task<ActionResult> MarcarLida(int id)
    {
        try
        {
            await _notificacaoService.MarcarLidaAsync(id);
            return Ok(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
    }
}
