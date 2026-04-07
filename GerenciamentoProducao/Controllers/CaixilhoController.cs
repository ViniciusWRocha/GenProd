using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using GerenciamentoProducaoo.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GerenciamentoProducaoo.Controllers
{
    public class CaixilhoController : Controller
    {
        private readonly ICaixilhoRepository _caixilhoRepository;
        private readonly IFamiliaCaixilhoRepository _familiaCaixilhoRepository;
        private readonly IObraRepository _obraRepository;

        public CaixilhoController(
            ICaixilhoRepository caixilhoRepository,
            IFamiliaCaixilhoRepository familiaCaixilhoRepository,
            IObraRepository obraRepository)
        {
            _caixilhoRepository = caixilhoRepository;
            _familiaCaixilhoRepository = familiaCaixilhoRepository;
            _obraRepository = obraRepository;
        }

        private async Task RecalcularProgressoObra(int obraId)
        {
            try { await _obraRepository.RecalcularProgressoAsync(obraId); } catch { }
        }

        private async Task<CaixilhoViewModel> CriarCaixilhoViewModel(CaixilhoViewModel? model = null)
        {
            var familiaCaixilhos = await _familiaCaixilhoRepository.GetAllAsync();
            var obras = await _obraRepository.GetAllAsync();

            int obraId = model?.ObraId ?? 0;
            int familiaId = model?.IdFamiliaCaixilho > 0 ? model.IdFamiliaCaixilho : (model?.FamiliaCaixilhoId ?? 0);

            return new CaixilhoViewModel
            {
                IdCaixilho = model?.IdCaixilho ?? 0,
                NomeCaixilho = model?.NomeCaixilho,
                Largura = model?.Largura ?? 0,
                Altura = model?.Altura ?? 0,
                Quantidade = model?.Quantidade ?? 0,
                PesoUnitario = model?.PesoUnitario ?? 0,
                ObraId = obraId,
                FamiliaCaixilhoId = familiaId,
                IdFamiliaCaixilho = familiaId,
                DescricaoCaixilho = model?.DescricaoCaixilho,
                Liberado = model?.Liberado ?? false,
                DataLiberacao = model?.DataLiberacao,
                StatusProducao = model?.StatusProducao ?? "Pendente",
                Observacoes = model?.Observacoes,
                Obra = obras.Select(o => new SelectListItem
                {
                    Value = o.IdObra.ToString(),
                    Text = o.Nome,
                    Selected = o.IdObra == obraId
                }),
                FamiliaCaixilho = familiaCaixilhos.Select(f => new SelectListItem
                {
                    Value = f.IdFamiliaCaixilho.ToString(),
                    Text = f.DescricaoFamilia,
                    Selected = f.IdFamiliaCaixilho == familiaId
                })
            };
        }

        public async Task<IActionResult> Index()
        {
            var caixilhos = await _caixilhoRepository.GetAllAsync();
            return View(caixilhos);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Create(int? familiaId)
        {
            if (familiaId.HasValue)
            {
                var familia = await _familiaCaixilhoRepository.GetByIdAsync(familiaId.Value);
                if (familia != null)
                {
                    var model = await CriarCaixilhoViewModel(new CaixilhoViewModel
                    {
                        ObraId = familia.IdObra,
                        IdFamiliaCaixilho = familiaId.Value,
                        FamiliaCaixilhoId = familiaId.Value
                    });
                    ViewBag.FamiliaId = familiaId.Value;
                    ViewBag.ObraId = familia.IdObra;
                    ViewBag.FamiliaNome = familia.DescricaoFamilia;
                    return View(model);
                }
            }
            var defaultModel = await CriarCaixilhoViewModel();
            return View(defaultModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CaixilhoViewModel caixilhoViewModel)
        {
            ModelState.Remove("Obra");
            ModelState.Remove("FamiliaCaixilho");

            if (caixilhoViewModel.IdFamiliaCaixilho <= 0)
            {
                if (caixilhoViewModel.FamiliaCaixilhoId > 0)
                    caixilhoViewModel.IdFamiliaCaixilho = caixilhoViewModel.FamiliaCaixilhoId;
                else
                    ModelState.AddModelError("IdFamiliaCaixilho", "E necessario selecionar uma familia de caixilho.");
            }

            if (ModelState.IsValid)
            {
                var caixilho = new Caixilho
                {
                    NomeCaixilho = caixilhoViewModel.NomeCaixilho,
                    Largura = caixilhoViewModel.Largura,
                    Altura = caixilhoViewModel.Altura,
                    Quantidade = caixilhoViewModel.Quantidade,
                    PesoUnitario = caixilhoViewModel.PesoUnitario,
                    ObraId = caixilhoViewModel.ObraId,
                    IdFamiliaCaixilho = caixilhoViewModel.IdFamiliaCaixilho,
                    DescricaoCaixilho = caixilhoViewModel.DescricaoCaixilho
                };
                await _caixilhoRepository.AddAsync(caixilho);
                if (caixilho.IdFamiliaCaixilho > 0)
                    return RedirectToAction("Details", "FamiliaCaixilho", new { id = caixilho.IdFamiliaCaixilho });
                return RedirectToAction(nameof(Index));
            }

            var model = await CriarCaixilhoViewModel(caixilhoViewModel);
            return View(model);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Edit(int id)
        {
            var caixilho = await _caixilhoRepository.GetById(id);
            if (caixilho == null) return NotFound();

            var familiaCaixilhos = await _familiaCaixilhoRepository.GetAllAsync();
            var obras = await _obraRepository.GetAllAsync();

            var model = new CaixilhoViewModel
            {
                IdCaixilho = caixilho.IdCaixilho,
                NomeCaixilho = caixilho.NomeCaixilho,
                Largura = caixilho.Largura,
                Altura = caixilho.Altura,
                Quantidade = caixilho.Quantidade,
                PesoUnitario = caixilho.PesoUnitario,
                ObraId = caixilho.ObraId,
                FamiliaCaixilhoId = caixilho.IdFamiliaCaixilho,
                IdFamiliaCaixilho = caixilho.IdFamiliaCaixilho,
                DescricaoCaixilho = caixilho.DescricaoCaixilho,
                Liberado = caixilho.Liberado,
                DataLiberacao = caixilho.DataLiberacao,
                StatusProducao = caixilho.StatusProducao,
                Observacoes = caixilho.Observacoes,
                Obra = obras.Select(o => new SelectListItem
                {
                    Value = o.IdObra.ToString(),
                    Text = o.Nome,
                    Selected = o.IdObra == caixilho.ObraId
                }),
                FamiliaCaixilho = familiaCaixilhos.Select(f => new SelectListItem
                {
                    Value = f.IdFamiliaCaixilho.ToString(),
                    Text = f.DescricaoFamilia,
                    Selected = f.IdFamiliaCaixilho == caixilho.IdFamiliaCaixilho
                })
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CaixilhoViewModel viewModel)
        {
            if (id != viewModel.IdCaixilho) return NotFound();

            ModelState.Remove("Obra");
            ModelState.Remove("FamiliaCaixilho");

            if (viewModel.IdFamiliaCaixilho <= 0)
            {
                if (viewModel.FamiliaCaixilhoId > 0)
                    viewModel.IdFamiliaCaixilho = viewModel.FamiliaCaixilhoId;
                else
                    ModelState.AddModelError("IdFamiliaCaixilho", "E necessario selecionar uma familia de caixilho.");
            }

            if (ModelState.IsValid)
            {
                var caixilho = await _caixilhoRepository.GetById(id);
                if (caixilho == null) return NotFound();

                caixilho.NomeCaixilho = viewModel.NomeCaixilho;
                caixilho.Largura = viewModel.Largura;
                caixilho.Altura = viewModel.Altura;
                caixilho.Quantidade = viewModel.Quantidade;
                caixilho.PesoUnitario = viewModel.PesoUnitario;
                caixilho.ObraId = viewModel.ObraId;
                caixilho.IdFamiliaCaixilho = viewModel.IdFamiliaCaixilho;
                caixilho.DescricaoCaixilho = viewModel.DescricaoCaixilho;
                caixilho.Liberado = viewModel.Liberado;
                caixilho.DataLiberacao = viewModel.DataLiberacao;
                caixilho.StatusProducao = viewModel.StatusProducao;
                caixilho.Observacoes = viewModel.Observacoes;

                await _caixilhoRepository.UpdateAsync(caixilho);
                if (caixilho.IdFamiliaCaixilho > 0)
                    return RedirectToAction("Details", "FamiliaCaixilho", new { id = caixilho.IdFamiliaCaixilho });
                return RedirectToAction(nameof(Index));
            }

            viewModel = await CriarCaixilhoViewModel(viewModel);
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var caixilho = await _caixilhoRepository.GetById(id.Value);
            if (caixilho == null) return NotFound();

            return View(caixilho);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _caixilhoRepository.GetById(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var caixilho = await _caixilhoRepository.GetById(id);
            var familiaId = caixilho?.IdFamiliaCaixilho;

            await _caixilhoRepository.DeleteAsync(id);

            if (familiaId.HasValue && familiaId.Value > 0)
                return RedirectToAction("Details", "FamiliaCaixilho", new { id = familiaId.Value });
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> MarcarMedido(int id, int familiaId)
        {
            var caixilho = await _caixilhoRepository.GetById(id);
            if (caixilho == null) return NotFound();

            caixilho.StatusProducao = "Medido";
            caixilho.Liberado = true;
            caixilho.DataLiberacao = DateTime.UtcNow;
            await _caixilhoRepository.UpdateAsync(caixilho);
            await RecalcularProgressoObra(caixilho.ObraId);

            TempData["SuccessMessage"] = $"Caixilho '{caixilho.NomeCaixilho}' marcado como medido!";
            if (familiaId > 0)
                return RedirectToAction("Details", "FamiliaCaixilho", new { id = familiaId });
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> DesmarcarMedido(int id, int familiaId)
        {
            var caixilho = await _caixilhoRepository.GetById(id);
            if (caixilho == null) return NotFound();

            caixilho.StatusProducao = "Pendente";
            caixilho.Liberado = false;
            caixilho.DataLiberacao = null;
            await _caixilhoRepository.UpdateAsync(caixilho);
            await RecalcularProgressoObra(caixilho.ObraId);

            TempData["SuccessMessage"] = $"Caixilho '{caixilho.NomeCaixilho}' voltou para pendente.";
            if (familiaId > 0)
                return RedirectToAction("Details", "FamiliaCaixilho", new { id = familiaId });
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Liberacoes(string? filtro)
        {
            filtro = filtro ?? "pendentes";
            var familias = await _familiaCaixilhoRepository.GetAllAsync();
            var resultado = new List<FamiliaCaixilho>();

            if (filtro == "liberadas")
            {
                resultado = familias.Where(f => f.StatusFamilia == "EmProducao" || f.StatusFamilia == "Produzida").ToList();
            }
            else if (filtro == "todas")
            {
                resultado = familias;
            }
            else // pendentes - prontas para liberar
            {
                foreach (var familia in familias)
                {
                    if (familia.StatusFamilia == "EmProducao" || familia.StatusFamilia == "Produzida")
                        continue;

                    var caixilhos = await _caixilhoRepository.GetByFamiliaIdAsync(familia.IdFamiliaCaixilho);
                    if (caixilhos.Any() && caixilhos.All(c => c.StatusProducao == "Medido"))
                    {
                        resultado.Add(familia);
                    }
                }
            }

            ViewBag.Filtro = filtro;
            ViewBag.Obras = (await _obraRepository.GetAllAsync()).ToDictionary(o => o.IdObra, o => o.Nome);
            return View(resultado);
        }
    }
}
