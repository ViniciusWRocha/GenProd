using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GerenciamentoProducao.Controllers
{
    public class FamiliaCaixilhoController : Controller
    {
        private readonly IFamiliaCaixilhoRepository _familiaCaixilhoRepository;
        private readonly IObraRepository _obraRepository;
        private readonly ICaixilhoRepository _caixilhoRepository;

        public FamiliaCaixilhoController(
            IFamiliaCaixilhoRepository familiaCaixilhoRepository,
            IObraRepository obraRepository,
            ICaixilhoRepository caixilhoRepository)
        {
            _familiaCaixilhoRepository = familiaCaixilhoRepository;
            _obraRepository = obraRepository;
            _caixilhoRepository = caixilhoRepository;
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

            return View(familia);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
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

            familia.StatusFamilia = "EmProducao";
            await _familiaCaixilhoRepository.UpdateAsync(familia);

            try { await _obraRepository.RecalcularProgressoAsync(familia.IdObra); } catch { }

            TempData["SuccessMessage"] = $"Família '{familia.DescricaoFamilia}' liberada para produção!";
            return RedirectToAction("Details", new { id });
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
