using System.Net.Http;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using GerenciamentoProducao.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace GerenciamentoProducao.Controllers
{
    public class FamiliaCaixilhoController : Controller
    {
        private const int MaxFotoBase64Length = 6_500_000;

        private readonly IFamiliaCaixilhoRepository _familiaCaixilhoRepository;
        private readonly IObraRepository _obraRepository;
        private readonly ICaixilhoRepository _caixilhoRepository;
        private readonly IFamiliaMedicaoFotoStore _medicaoFotoStore;
        private readonly MedicaoApiService _medicaoApiService;
        private readonly NotificacaoApiService _notificacaoService;

        public FamiliaCaixilhoController(
            IFamiliaCaixilhoRepository familiaCaixilhoRepository,
            IObraRepository obraRepository,
            ICaixilhoRepository caixilhoRepository,
            IFamiliaMedicaoFotoStore medicaoFotoStore,
            MedicaoApiService medicaoApiService,
            NotificacaoApiService notificacaoService)
        {
            _familiaCaixilhoRepository = familiaCaixilhoRepository;
            _obraRepository = obraRepository;
            _caixilhoRepository = caixilhoRepository;
            _medicaoFotoStore = medicaoFotoStore;
            _medicaoApiService = medicaoApiService;
            _notificacaoService = notificacaoService;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _familiaCaixilhoRepository.GetAllAsync();
            var obras = await _obraRepository.GetAllAsync();
            ViewBag.Obras = obras.ToDictionary(o => o.IdObra, o => o.Nome);
            return View(lista);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Create(int? obraId)
        {
            if (obraId.HasValue)
            {
                var obra = await _obraRepository.GetById(obraId.Value);
                if (obra != null)
                {
                    ViewBag.ObraId = obraId.Value;
                    ViewBag.ObraNome = obra.Nome;
                    return View(new FamiliaCaixilho { IdObra = obraId.Value });
                }
            }
            await PopularObrasViewBag();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FamiliaCaixilho familia)
        {
            if (ModelState.IsValid)
            {
                familia.PesoTotal = 0;
                await _familiaCaixilhoRepository.AddAsync(familia);
                return RedirectToAction("Details", "Obra", new { id = familia.IdObra });
            }
            if (familia.IdObra > 0)
            {
                var obra = await _obraRepository.GetById(familia.IdObra);
                ViewBag.ObraId = familia.IdObra;
                ViewBag.ObraNome = obra?.Nome;
            }
            else
            {
                await PopularObrasViewBag();
            }
            return View(familia);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<ActionResult> Edit(int id)
        {
            var familia = await _familiaCaixilhoRepository.GetByIdAsync(id);
            if (familia == null) return NotFound();
            await PopularObrasViewBag(familia.IdObra);
            return View(familia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FamiliaCaixilho familia)
        {
            if (id != familia.IdFamiliaCaixilho) return NotFound();

            if (ModelState.IsValid)
            {
                await _familiaCaixilhoRepository.UpdateAsync(familia);
                return RedirectToAction("Details", "Obra", new { id = familia.IdObra });
            }
            await PopularObrasViewBag(familia.IdObra);
            return View(familia);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _familiaCaixilhoRepository.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var familia = await _familiaCaixilhoRepository.GetByIdAsync(id);
            var obraId = familia?.IdObra;

            await _familiaCaixilhoRepository.DeleteAsync(id);

            if (obraId.HasValue)
                return RedirectToAction("Details", "Obra", new { id = obraId.Value });
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var familia = await _familiaCaixilhoRepository.GetByIdAsync(id.Value);
            if (familia == null) return NotFound();

            var caixilhos = await _caixilhoRepository.GetByFamiliaIdAsync(id.Value);
            ViewBag.Caixilhos = caixilhos;

            var obra = await _obraRepository.GetById(familia.IdObra);
            ViewBag.ObraNome = obra?.Nome;
            ViewBag.MedicaoFotoPendente = await _medicaoFotoStore.GetAsync(familia.IdFamiliaCaixilho);

            // Dados de produção e medição da API
            ViewBag.ProducaoFamilia = await _medicaoApiService.GetProducaoByFamiliaAsync(id.Value);
            ViewBag.MedicaoFamilia = await _medicaoApiService.GetByFamiliaAsync(id.Value);

            return View(familia);
        }

        /// <summary>Envio da foto da medição (família de caixilhos). Qualquer utilizador autenticado. Foto vem antes de «Medido»; aprovação na web marca como medido.</summary>
        public async Task<IActionResult> EnviarFotoMedicao(int id)
        {
            var familia = await _familiaCaixilhoRepository.GetByIdAsync(id);
            if (familia == null) return NotFound();

            if (familia.StatusFamilia == "EmProducao" || familia.StatusFamilia == "Produzida")
            {
                TempData["ErrorMessage"] = "Esta família já está em produção ou produzida.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var caixilhos = await _caixilhoRepository.GetByFamiliaIdAsync(id);
            if (!caixilhos.Any())
            {
                TempData["ErrorMessage"] = "Cadastre caixilhos nesta família antes de enviar a foto.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (caixilhos.All(c => c.StatusProducao == "Medido"))
            {
                TempData["ErrorMessage"] = "A medição desta família já foi confirmada (todos os caixilhos estão medidos).";
                return RedirectToAction(nameof(Details), new { id });
            }

            var fotoExistente = await _medicaoFotoStore.GetAsync(id);
            if (fotoExistente != null && !fotoExistente.Aprovada)
            {
                TempData["ErrorMessage"] = "Já existe uma foto aguardando aprovação. Aguarde ou peça a rejeição para enviar outra.";
                return RedirectToAction(nameof(Details), new { id });
            }

            ViewBag.FamiliaNome = familia.DescricaoFamilia;
            ViewBag.IdFamilia = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EnviarFotoMedicao(int id, string fotoBase64)
        {
            var familia = await _familiaCaixilhoRepository.GetByIdAsync(id);
            if (familia == null) return NotFound();

            if (familia.StatusFamilia == "EmProducao" || familia.StatusFamilia == "Produzida")
            {
                TempData["ErrorMessage"] = "Esta família já está em produção ou produzida.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var caixilhos = await _caixilhoRepository.GetByFamiliaIdAsync(id);
            if (!caixilhos.Any())
            {
                TempData["ErrorMessage"] = "Não há caixilhos nesta família.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (caixilhos.All(c => c.StatusProducao == "Medido"))
            {
                TempData["ErrorMessage"] = "A medição já está confirmada.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var fotoExistentePost = await _medicaoFotoStore.GetAsync(id);
            if (fotoExistentePost != null && !fotoExistentePost.Aprovada)
            {
                TempData["ErrorMessage"] = "Já existe foto pendente de aprovação.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (string.IsNullOrWhiteSpace(fotoBase64))
            {
                TempData["ErrorMessage"] = "É necessário capturar ou selecionar uma foto.";
                return RedirectToAction(nameof(EnviarFotoMedicao), new { id });
            }

            fotoBase64 = fotoBase64.Trim();
            if (fotoBase64.Length > MaxFotoBase64Length)
            {
                TempData["ErrorMessage"] = "A imagem é demasiado grande. Tente reduzir a resolução.";
                return RedirectToAction(nameof(EnviarFotoMedicao), new { id });
            }

            if (!fotoBase64.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
                fotoBase64 = "data:image/jpeg;base64," + fotoBase64;

            var state = new FamiliaMedicaoFotoState
            {
                IdFamiliaCaixilho = id,
                FotoBase64 = fotoBase64,
                EnviadoEm = DateTime.UtcNow,
                EnviadoPor = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? User.Identity?.Name
            };
            await _medicaoFotoStore.SaveAsync(state);

            TempData["SuccessMessage"] = "Foto enviada. Aguarde aprovação na web: se for aprovada, os caixilhos passam a Medido; se for rejeitada, pode enviar outra foto.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> AprovarMedicaoFoto(int id)
        {
            var pendente = await _medicaoFotoStore.GetAsync(id);
            if (pendente == null)
            {
                TempData["ErrorMessage"] = "Não há foto pendente de aprovação para esta família.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var familia = await _familiaCaixilhoRepository.GetByIdAsync(id);
            if (familia == null) return NotFound();

            var caixilhos = await _caixilhoRepository.GetByFamiliaIdAsync(id);
            if (!caixilhos.Any())
            {
                TempData["ErrorMessage"] = "Não há caixilhos nesta família.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (caixilhos.All(c => c.StatusProducao == "Medido"))
            {
                pendente.Aprovada = true;
                await _medicaoFotoStore.SaveAsync(pendente);
                TempData["ErrorMessage"] = "Os caixilhos já estão medidos; foto marcada como aprovada.";
                return RedirectToAction(nameof(Details), new { id });
            }

            pendente.Aprovada = true;
            await _medicaoFotoStore.SaveAsync(pendente);

            foreach (var c in caixilhos)
            {
                if (c.StatusProducao == "Medido")
                    continue;
                c.StatusProducao = "Medido";
                c.Liberado = true;
                c.DataLiberacao = DateTime.UtcNow;
                await _caixilhoRepository.UpdateAsync(c);
            }

            familia.StatusFamilia = "Medida";
            await _familiaCaixilhoRepository.UpdateAsync(familia);

            try { await _familiaCaixilhoRepository.AtualizarPesoTotalAsync(familia.IdFamiliaCaixilho); } catch { }
            try { await _obraRepository.RecalcularProgressoAsync(familia.IdObra); } catch { }

            // Notificar medidores e produtores que a foto foi aprovada
            // TipoNotificacao.FotoMedicaoAprovada = 7
            // TipoCargo.ResponsavelMedicao = 3, ResponsavelProducao = 4
            _ = Task.WhenAll(
                _notificacaoService.BroadcastAsync(
                    "Medição aprovada",
                    $"A foto de medição da família \"{familia.DescricaoFamilia}\" foi aprovada. A família está pronta para produção.",
                    7, familia.IdObra, 3),
                _notificacaoService.BroadcastAsync(
                    "Medição aprovada",
                    $"A foto de medição da família \"{familia.DescricaoFamilia}\" foi aprovada. A família está pronta para produção.",
                    7, familia.IdObra, 4)
            );

            TempData["SuccessMessage"] = $"Medição aprovada. Todos os caixilhos da família '{familia.DescricaoFamilia}' foram marcados como Medido. Pode liberar para produção quando desejar.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> RejeitarMedicaoFoto(int id)
        {
            var pendente = await _medicaoFotoStore.GetAsync(id);
            if (pendente == null)
            {
                TempData["ErrorMessage"] = "Não há foto pendente para rejeitar.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await _medicaoFotoStore.DeleteAsync(id);

            TempData["SuccessMessage"] = "Foto rejeitada e removida. Pode enviar uma nova foto da medição.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> LiberarProducao(int id)
        {
            var familia = await _familiaCaixilhoRepository.GetByIdAsync(id);
            if (familia == null) return NotFound();

            var caixilhos = await _caixilhoRepository.GetByFamiliaIdAsync(id);
            if (!caixilhos.Any() || !caixilhos.All(c => c.StatusProducao == "Medido"))
            {
                TempData["ErrorMessage"] = "Todos os caixilhos devem estar medidos para liberar a produção.";
                return RedirectToAction("Details", new { id });
            }

            await _medicaoFotoStore.DeleteAsync(id);

            familia.StatusFamilia = "EmProducao";
            await _familiaCaixilhoRepository.UpdateAsync(familia);

            try { await _obraRepository.RecalcularProgressoAsync(familia.IdObra); } catch { }

            TempData["SuccessMessage"] = $"Família '{familia.DescricaoFamilia}' liberada para produção!";
            return RedirectToAction("Details", new { id });
        }

        /// <summary>Família em EmProducao → Produzida. Atualiza percentuais da obra (famílias concluídas / total).</summary>
        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> FinalizarProducaoFamilia(int id)
        {
            var familia = await _familiaCaixilhoRepository.GetByIdAsync(id);
            if (familia == null) return NotFound();

            if (familia.StatusFamilia != "EmProducao")
            {
                TempData["ErrorMessage"] = "Só é possível finalizar quando a família está liberada e em produção (ainda não concluída).";
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                await _familiaCaixilhoRepository.FinalizarProducaoAsync(id);
                try { await _obraRepository.RecalcularProgressoAsync(familia.IdObra); } catch { }
                TempData["SuccessMessage"] =
                    $"Produção da família '{familia.DescricaoFamilia}' concluída. O progresso da obra foi atualizado.";
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Não foi possível finalizar na API: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> RecalcularPesos()
        {
            try
            {
                var familias = await _familiaCaixilhoRepository.GetAllAsync();
                int familiasAtualizadas = 0;

                foreach (var familia in familias)
                {
                    await _familiaCaixilhoRepository.AtualizarPesoTotalAsync(familia.IdFamiliaCaixilho);
                    familiasAtualizadas++;
                }

                TempData["SuccessMessage"] = $"Pesos recalculados com sucesso para {familiasAtualizadas} familias.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao recalcular pesos: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task PopularObrasViewBag(int? selectedObraId = null)
        {
            var obras = await _obraRepository.GetAllAsync();
            ViewBag.Obras = new SelectList(obras, "IdObra", "Nome", selectedObraId);
        }
    }
}
