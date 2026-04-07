using API.SIGE.DTOs;
using API.SIGE.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.SIGE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardApiController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardApiController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("metricas")]
    public async Task<ActionResult<DashboardMetricasDto>> GetMetricas()
    {
        var metricas = await _dashboardService.GetMetricasAsync();
        return Ok(metricas);
    }

    [HttpPost("atualizar-progresso-obras")]
    public async Task<ActionResult> AtualizarProgressoObras()
    {
        await _dashboardService.AtualizarProgressoObrasAsync();
        return Ok(new { success = true });
    }
}
