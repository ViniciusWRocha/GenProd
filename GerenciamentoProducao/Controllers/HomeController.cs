using System.Diagnostics;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using GerenciamentoProducao.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoProducao.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IObraRepository _obraRepository;
        private readonly IFamiliaCaixilhoRepository _familiaCaixilhoRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IObraRepository obraRepository,
            IFamiliaCaixilhoRepository familiaCaixilhoRepository)
        {
            _logger = logger;
            _obraRepository = obraRepository;
            _familiaCaixilhoRepository = familiaCaixilhoRepository;
        }

        public async Task<IActionResult> Index()
        {
            if (!User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Login", "Usuario");
            }

            try
            {
                var obras = await _obraRepository.GetAllAsync();
                var familias = await _familiaCaixilhoRepository.GetAllAsync();

                var obrasConcluidas = obras.Count(o => o.Finalizado || o.StatusObra == "Concluida");
                var obrasEmAndamento = obras.Where(o => !o.Finalizado && o.StatusObra != "Concluida").ToList();

                var familiasLiberadas = familias.Count(f => f.StatusFamilia == "EmProducao" || f.StatusFamilia == "Produzida");

                float pesoLiberado = 0, pesoPendente = 0;
                foreach (var f in familias)
                {
                    if (f.StatusFamilia == "EmProducao" || f.StatusFamilia == "Produzida")
                        pesoLiberado += f.PesoTotal;
                    else
                        pesoPendente += f.PesoTotal;
                }

                var hoje = DateTime.Now;
                int emDia = 0, emAlerta = 0, atrasadas = 0;

                var obrasItems = obrasEmAndamento
                    .OrderByDescending(o => o.IdObra)
                    .Select(o =>
                    {
                        var situacao = CalcularSituacaoPrazo(o.DataInicio, o.DataTermino, hoje);
                        if (situacao == "Atrasada") atrasadas++;
                        else if (situacao == "Alerta") emAlerta++;
                        else emDia++;

                        return new ObraDashboardItem
                        {
                            Id = o.IdObra,
                            Nome = o.Nome,
                            Construtora = o.Construtora,
                            StatusObra = o.StatusObra,
                            SituacaoPrazo = situacao,
                            PercentualMedicao = o.PercentualMedicao,
                            PercentualProducao = o.PercentualProducao,
                            PesoFinal = o.PesoFinal,
                            DataInicio = o.DataInicio,
                            DataTermino = o.DataTermino,
                            DiasRestantes = (int)(o.DataTermino - hoje).TotalDays
                        };
                    })
                    .ToList();

                var dashboard = new DashboardViewModel
                {
                    TotalObras = obras.Count,
                    ObrasConcluidas = obrasConcluidas,
                    FamiliasLiberadas = familiasLiberadas,
                    TotalFamilias = familias.Count,
                    PesoLiberadoKg = pesoLiberado,
                    PesoPendenteKg = pesoPendente,
                    ObrasEmDia = emDia,
                    ObrasEmAlerta = emAlerta,
                    ObrasAtrasadas = atrasadas,
                    Obras = obrasItems
                };

                return View(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dashboard");
                return View(new DashboardViewModel());
            }
        }

        private static string CalcularSituacaoPrazo(DateTime dataInicio, DateTime dataTermino, DateTime hoje)
        {
            if (hoje > dataTermino)
                return "Atrasada";

            var prazoTotal = (dataTermino - dataInicio).TotalDays;
            if (prazoTotal <= 0)
                return "Alerta";

            var diasDecorridos = (hoje - dataInicio).TotalDays;
            var percentualDecorrido = diasDecorridos / prazoTotal * 100;

            return percentualDecorrido >= 75 ? "Alerta" : "EmDia";
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
