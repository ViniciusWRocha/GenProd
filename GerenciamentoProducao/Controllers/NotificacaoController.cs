using GerenciamentoProducao.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoProducao.Controllers;

public class NotificacaoController : Controller
{
    private readonly NotificacaoApiService _notificacaoService;

    public NotificacaoController(NotificacaoApiService notificacaoService)
    {
        _notificacaoService = notificacaoService;
    }

    private int GetUsuarioId()
    {
        var claim = User.FindFirst("IdUsuario")?.Value;
        return int.TryParse(claim, out var id) ? id : 0;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var idUsuario = GetUsuarioId();
        if (idUsuario == 0) return RedirectToAction("Login", "Usuario");

        var notificacoes = await _notificacaoService.GetByUsuarioAsync(idUsuario);
        return View(notificacoes);
    }

    [HttpGet("api/notificacoes/count")]
    public async Task<IActionResult> NaoLidasCount()
    {
        var idUsuario = GetUsuarioId();
        if (idUsuario == 0) return Json(new { count = 0 });

        var count = await _notificacaoService.GetNaoLidasCountAsync(idUsuario);
        return Json(new { count });
    }

    [HttpPost]
    public async Task<IActionResult> MarcarLida(int id)
    {
        await _notificacaoService.MarcarLidaAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
