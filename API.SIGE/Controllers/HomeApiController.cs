using API.SIGE.DTOs;
using API.SIGE.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.SIGE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeApiController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly IObraService _obraService;

    public HomeApiController(IDashboardService dashboardService, IObraService obraService)
    {
        _dashboardService = dashboardService;
        _obraService = obraService;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardMetricasDto>> Dashboard()
    {
        var metricas = await _dashboardService.GetMetricasAsync();
        return Ok(metricas);
    }

    [HttpPost("importar-obras-xml")]
    public async Task<ActionResult> ImportarObrasXml([FromForm] List<IFormFile> arquivosXml)
    {
        if (arquivosXml == null || arquivosXml.Count == 0)
        {
            return BadRequest(new { success = false, message = "Nenhum arquivo XML enviado." });
        }

        var resultados = await _obraService.ImportarObrasXmlAsync(arquivosXml);
        return Ok(new { success = true, resultados });
    }
}
