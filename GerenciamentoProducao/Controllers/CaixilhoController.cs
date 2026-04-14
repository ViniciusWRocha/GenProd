using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using GerenciamentoProducao.Services;
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
        private readonly IFamiliaMedicaoFotoStore _medicaoFotoStore;
        private readonly MedicaoApiService _medicaoApiService;

        public CaixilhoController(
            ICaixilhoRepository caixilhoRepository,
            IFamiliaCaixilhoRepository familiaCaixilhoRepository,
            IObraRepository obraRepository,
            IFamiliaMedicaoFotoStore medicaoFotoStore,
            MedicaoApiService medicaoApiService)
        {
            _caixilhoRepository = caixilhoRepository;
            _familiaCaixilhoRepository = familiaCaixilhoRepository;
            _obraRepository = obraRepository;
            _medicaoFotoStore = medicaoFotoStore;
            _medicaoApiService = medicaoApiService;
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
                resultado = familias.Where(f => f.StatusFamilia == "EmProducao").ToList();
            }
            else if (filtro == "concluidas")
            {
                resultado = familias.Where(f => f.StatusFamilia == "Produzida").ToList();
            }
            else if (filtro == "todas")
            {
                resultado = familias;
            }
            else // pendentes: prontas para liberar (todos medidos) ou com foto aguardando aprovação
            {
                var mapa = new Dictionary<int, FamiliaCaixilho>();
                foreach (var familia in familias)
                {
                    if (familia.StatusFamilia == "EmProducao" || familia.StatusFamilia == "Produzida")
                        continue;

                    var caixilhos = await _caixilhoRepository.GetByFamiliaIdAsync(familia.IdFamiliaCaixilho);
                    if (caixilhos.Any() && caixilhos.All(c => c.StatusProducao == "Medido"))
                        mapa[familia.IdFamiliaCaixilho] = familia;
                }

                foreach (var idPend in await _medicaoFotoStore.ListFamiliasComFotoPendenteAsync())
                {
                    var f = familias.FirstOrDefault(x => x.IdFamiliaCaixilho == idPend);
                    if (f != null && f.StatusFamilia != "EmProducao" && f.StatusFamilia != "Produzida")
                        mapa[idPend] = f;
                }

                resultado = mapa.Values.OrderBy(f => f.DescricaoFamilia).ToList();
            }

            var medicaoPendenteIds = new HashSet<int>();
            foreach (var f in resultado)
            {
                var foto = await _medicaoFotoStore.GetAsync(f.IdFamiliaCaixilho);
                if (foto != null && !foto.Aprovada)
                    medicaoPendenteIds.Add(f.IdFamiliaCaixilho);
            }

            var podeEnviarFotoIds = new HashSet<int>();
            foreach (var familia in familias)
            {
                if (familia.StatusFamilia == "EmProducao" || familia.StatusFamilia == "Produzida")
                    continue;
                var caixilhos = await _caixilhoRepository.GetByFamiliaIdAsync(familia.IdFamiliaCaixilho);
                if (caixilhos.Any() && !caixilhos.All(c => c.StatusProducao == "Medido"))
                    podeEnviarFotoIds.Add(familia.IdFamiliaCaixilho);
            }

            ViewBag.Filtro = filtro;
            ViewBag.Obras = (await _obraRepository.GetAllAsync()).ToDictionary(o => o.IdObra, o => o.Nome);
            ViewBag.MedicaoPendenteIds = medicaoPendenteIds;
            ViewBag.PodeEnviarFotoIds = podeEnviarFotoIds;
            return View(resultado);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> DetalheMedicao(int familiaId)
        {
            var familia = await _familiaCaixilhoRepository.GetByIdAsync(familiaId);
            if (familia == null)
                return Json(new { success = false, message = "Família não encontrada." });

            // Foto local (pendente ou aprovada)
            string? fotoLocalBase64 = null;
            string? fotoLocalEnviadoPor = null;
            DateTime? fotoLocalEnviadoEm = null;
            bool fotoAprovada = false;
            var fotoLocal = await _medicaoFotoStore.GetAsync(familiaId);
            if (fotoLocal != null)
            {
                fotoLocalBase64 = fotoLocal.FotoBase64;
                fotoLocalEnviadoPor = fotoLocal.EnviadoPor;
                fotoLocalEnviadoEm = fotoLocal.EnviadoEm;
                fotoAprovada = fotoLocal.Aprovada;
            }

            // Medição da API
            var medicao = await _medicaoApiService.GetByFamiliaAsync(familiaId);

            // Fotos da API (Anexos vinculados à medição)
            var fotosApi = new List<object>();
            if (medicao != null)
            {
                var anexos = await _medicaoApiService.GetAnexosByMedicaoAsync(medicao.IdMedicao);
                foreach (var anexo in anexos)
                {
                    // Somente imagens
                    if (!anexo.TipoArquivo.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var bytes = await _medicaoApiService.DownloadAnexoAsync(anexo.IdAnexo);
                    if (bytes != null)
                    {
                        var base64 = $"data:{anexo.TipoArquivo};base64,{Convert.ToBase64String(bytes)}";
                        fotosApi.Add(new
                        {
                            nomeArquivo = anexo.NomeArquivo,
                            base64 = base64,
                            dataUpload = anexo.DataUpload.ToString("dd/MM/yyyy HH:mm"),
                            nomeUsuario = anexo.NomeUsuario
                        });
                    }
                }
            }

            // Produção da API
            var producao = await _medicaoApiService.GetProducaoByFamiliaAsync(familiaId);

            return Json(new
            {
                success = true,
                familiaDescricao = familia.DescricaoFamilia,
                // Medição API
                temMedicao = medicao != null,
                medicaoStatus = medicao?.StatusTexto,
                medicaoResponsavel = medicao?.NomeResponsavel,
                medicaoDataInicio = medicao?.DataInicio?.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                medicaoDataConclusao = medicao?.DataConclusao?.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                medicaoDescricao = medicao?.Descricao,
                medicaoObservacoes = medicao?.Observacoes,
                // Foto local
                temFotoLocal = fotoLocal != null,
                fotoLocalBase64,
                fotoLocalEnviadoPor,
                fotoLocalEnviadoEm = fotoLocalEnviadoEm?.ToString("dd/MM/yyyy HH:mm"),
                fotoAprovada,
                // Fotos da API
                fotosApi,
                // Produção API
                temProducao = producao != null,
                producaoStatus = producao?.StatusTexto,
                producaoResponsavel = producao?.NomeResponsavel,
                producaoDataInicio = producao?.DataInicio?.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                producaoDataConclusao = producao?.DataConclusao?.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                producaoPrevisao = producao?.DataEstimadaConclusao?.ToString("dd/MM/yyyy"),
                producaoDescricao = producao?.Descricao,
                producaoObservacoes = producao?.Observacoes
            });
        }
    }
}
