using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using GerenciamentoProducaoo.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GerenciamentoProducaoo.Controllers
{
    public class ObraController : Controller
    {
        private readonly IObraRepository _obraRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IFamiliaCaixilhoRepository _familiaCaixilhoRepository;
        private readonly ICaixilhoRepository _caixilhoRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static readonly string[] _extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private const long _tamanhoMaximoImagemBytes = 5 * 1024 * 1024; // 5MB

        public ObraController(
            IObraRepository obraRepository,
            IUsuarioRepository usuarioRepository,
            IFamiliaCaixilhoRepository familiaCaixilhoRepository,
            ICaixilhoRepository caixilhoRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _obraRepository = obraRepository;
            _usuarioRepository = usuarioRepository;
            _familiaCaixilhoRepository = familiaCaixilhoRepository;
            _caixilhoRepository = caixilhoRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        private async Task<ObraViewModel> CriarObraViewModel(ObraViewModel? model = null)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();

            return new ObraViewModel
            {
                IdObra = model?.IdObra ?? 0,
                Nome = model?.Nome,
                Construtora = model?.Construtora,
                Nro = model?.Nro,
                Logradouro = model?.Logradouro,
                Bairro = model?.Bairro,
                Cep = model?.Cep,
                Uf = model?.Uf,
                Cnpj = model?.Cnpj,
                DataInicio = model?.DataInicio ?? DateTime.Now,
                DataTermino = model?.DataTermino ?? DateTime.Now.AddMonths(6),
                PesoFinal = model?.PesoFinal ?? 0,
                PesoProduzido = model?.PesoProduzido ?? 0,
                StatusObra = model?.StatusObra ?? "Cadastrada",
                PercentualConclusao = model?.PercentualConclusao ?? 0,
                DataConclusao = model?.DataConclusao,
                Observacoes = model?.Observacoes,
                Finalizado = model?.Finalizado ?? false,
                IdUsuario = model?.IdUsuario ?? 0,
                ImagemObraPath = model?.ImagemObraPath,
                IdResponsavelVerificacao = model?.IdResponsavelVerificacao,
                IdResponsavelMedicao = model?.IdResponsavelMedicao,
                IdResponsavelProducao = model?.IdResponsavelProducao,

                Usuario = usuarios.Select(t => new SelectListItem
                {
                    Value = t.IdUsuario.ToString(),
                    Text = t.NomeUsuario,
                    Selected = model != null && t.IdUsuario == model.IdUsuario
                }),
                ResponsaveisVerificacao = usuarios.Select(t => new SelectListItem
                {
                    Value = t.IdUsuario.ToString(),
                    Text = t.NomeUsuario,
                    Selected = model != null && t.IdUsuario == model.IdResponsavelVerificacao
                }),
                ResponsaveisMedicao = usuarios.Select(t => new SelectListItem
                {
                    Value = t.IdUsuario.ToString(),
                    Text = t.NomeUsuario,
                    Selected = model != null && t.IdUsuario == model.IdResponsavelMedicao
                }),
                ResponsaveisProducao = usuarios.Select(t => new SelectListItem
                {
                    Value = t.IdUsuario.ToString(),
                    Text = t.NomeUsuario,
                    Selected = model != null && t.IdUsuario == model.IdResponsavelProducao
                })
            };
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? filtro, int? IdUsuario, string? search)
        {
            filtro = filtro ?? "andamento";

            List<Obra> obras;
            if (filtro == "concluidas")
                obras = await _obraRepository.GetAllFinalizadosAsync();
            else if (filtro == "todas")
                obras = await _obraRepository.GetAllAsync();
            else
                obras = await _obraRepository.GetAllNaoFinalizadosAsync();

            if (IdUsuario.HasValue && IdUsuario.Value > 0)
            {
                obras = obras.Where(o => o.IdUsuario == IdUsuario.Value).ToList();
            }
            if (!string.IsNullOrEmpty(search))
            {
                obras = obras.Where(o => o.Nome.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            obras = obras.OrderByDescending(o => o.IdObra).ToList();

            ViewBag.Filtro = filtro;
            ViewBag.Usuarios = new SelectList(await _usuarioRepository.GetAllAsync(), "IdUsuario", "NomeUsuario");
            ViewBag.TermoBusca = search;

            return View(obras);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        [HttpGet]
        public async Task<IActionResult> Finalizados()
        {
            var obras = await _obraRepository.GetAllFinalizadosAsync();
            obras = obras.OrderByDescending(o => o.IdObra).ToList();
            return View(obras);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Create()
        {
            var model = await CriarObraViewModel();
            return View(model);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        [HttpPost]
        public async Task<IActionResult> Create(ObraViewModel viewModel)
        {
            if (viewModel.ImagemUpload != null && viewModel.ImagemUpload.Length > 0 &&
                !ValidarImagem(viewModel.ImagemUpload, out var erroImagemCreate))
            {
                ModelState.AddModelError(nameof(viewModel.ImagemUpload), erroImagemCreate!);
            }

            if (!ModelState.IsValid)
            {
                var vm = await CriarObraViewModel(viewModel);
                return View(vm);
            }

            var obra = new Obra
            {
                Nome = viewModel.Nome,
                Construtora = viewModel.Construtora,
                Nro = viewModel.Nro,
                Logradouro = viewModel.Logradouro,
                Bairro = viewModel.Bairro,
                Cep = viewModel.Cep,
                Uf = viewModel.Uf,
                Cnpj = viewModel.Cnpj,
                DataInicio = viewModel.DataInicio,
                DataTermino = viewModel.DataTermino,
                PesoFinal = 0,
                PesoProduzido = 0,
                StatusObra = "Cadastrada",
                PercentualConclusao = viewModel.PercentualConclusao,
                DataConclusao = viewModel.DataConclusao,
                Observacoes = viewModel.Observacoes,
                Finalizado = viewModel.Finalizado,
                IdUsuario = viewModel.IdUsuario,
                ImagemObraPath = viewModel.ImagemObraPath,
                IdResponsavelVerificacao = viewModel.IdResponsavelVerificacao,
                IdResponsavelMedicao = viewModel.IdResponsavelMedicao,
                IdResponsavelProducao = viewModel.IdResponsavelProducao
            };

            // Imagem local (temporário até integrar com API de Anexos)
            var imagemSalva = await SalvarImagemAsync(viewModel.ImagemUpload);
            if (!string.IsNullOrEmpty(imagemSalva))
            {
                obra.ImagemObraPath = imagemSalva;
            }

            await _obraRepository.AddAsync(obra);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador,Gerente")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return NotFound();

            var item = await _obraRepository.GetById(id);
            if (item == null) return NotFound();

            var usuarios = await _usuarioRepository.GetAllAsync();

            var vm = new ObraViewModel
            {
                IdObra = item.IdObra,
                Nome = item.Nome,
                Construtora = item.Construtora,
                Nro = item.Nro,
                Logradouro = item.Logradouro,
                Bairro = item.Bairro,
                Cep = item.Cep,
                Uf = item.Uf,
                Cnpj = item.Cnpj,
                DataInicio = item.DataInicio,
                DataTermino = item.DataTermino,
                PesoFinal = item.PesoFinal,
                PesoProduzido = item.PesoProduzido,
                StatusObra = item.StatusObra,
                PercentualConclusao = item.PercentualConclusao,
                DataConclusao = item.DataConclusao,
                Observacoes = item.Observacoes,
                Finalizado = item.Finalizado,
                IdUsuario = item.IdUsuario,
                ImagemObraPath = item.ImagemObraPath,
                IdResponsavelVerificacao = item.IdResponsavelVerificacao,
                IdResponsavelMedicao = item.IdResponsavelMedicao,
                IdResponsavelProducao = item.IdResponsavelProducao,
                Usuario = usuarios.Select(t => new SelectListItem
                {
                    Value = t.IdUsuario.ToString(),
                    Text = t.NomeUsuario
                }),
                ResponsaveisVerificacao = usuarios.Select(t => new SelectListItem
                {
                    Value = t.IdUsuario.ToString(),
                    Text = t.NomeUsuario
                }),
                ResponsaveisMedicao = usuarios.Select(t => new SelectListItem
                {
                    Value = t.IdUsuario.ToString(),
                    Text = t.NomeUsuario
                }),
                ResponsaveisProducao = usuarios.Select(t => new SelectListItem
                {
                    Value = t.IdUsuario.ToString(),
                    Text = t.NomeUsuario
                })
            };

            return View(vm);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ObraViewModel viewModel)
        {
            if (id != viewModel.IdObra) return NotFound();

            if (viewModel.ImagemUpload != null && viewModel.ImagemUpload.Length > 0 &&
                !ValidarImagem(viewModel.ImagemUpload, out var erroImagemEdit))
            {
                ModelState.AddModelError(nameof(viewModel.ImagemUpload), erroImagemEdit!);
            }

            if (!ModelState.IsValid)
            {
                viewModel.Usuario = (await _usuarioRepository.GetAllAsync())
                    .Select(t => new SelectListItem
                    {
                        Value = t.IdUsuario.ToString(),
                        Text = t.NomeUsuario
                    });
                return View(viewModel);
            }

            var obra = await _obraRepository.GetById(id);
            if (obra == null) return NotFound();

            obra.Nome = viewModel.Nome;
            obra.Construtora = viewModel.Construtora;
            obra.Nro = viewModel.Nro;
            obra.Logradouro = viewModel.Logradouro;
            obra.Bairro = viewModel.Bairro;
            obra.Cep = viewModel.Cep;
            obra.Uf = viewModel.Uf;
            obra.Cnpj = viewModel.Cnpj;
            obra.DataInicio = viewModel.DataInicio;
            obra.DataTermino = viewModel.DataTermino;
            // PesoFinal é calculado automaticamente - não sobrescrever
            obra.StatusObra = viewModel.StatusObra;
            obra.PercentualConclusao = viewModel.PercentualConclusao;
            obra.DataConclusao = viewModel.DataConclusao;
            obra.Observacoes = viewModel.Observacoes;
            obra.IdUsuario = viewModel.IdUsuario;
            obra.IdResponsavelVerificacao = viewModel.IdResponsavelVerificacao;
            obra.IdResponsavelMedicao = viewModel.IdResponsavelMedicao;
            obra.IdResponsavelProducao = viewModel.IdResponsavelProducao;

            if (viewModel.ImagemUpload != null && viewModel.ImagemUpload.Length > 0)
            {
                var imagemSalva = await SalvarImagemAsync(viewModel.ImagemUpload);
                if (!string.IsNullOrEmpty(imagemSalva))
                {
                    RemoverImagemFisica(obra.ImagemObraPath);
                    obra.ImagemObraPath = imagemSalva;
                }
            }
            else
            {
                obra.ImagemObraPath = viewModel.ImagemObraPath;
            }

            await _obraRepository.UpdateAsync(obra);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var obra = await _obraRepository.GetById(id.Value);
            if (obra == null) return NotFound();

            var familias = await _familiaCaixilhoRepository.GetByObraIdAsync(id.Value);
            var todosOsCaixilhos = new List<Caixilho>();
            foreach (var familia in familias)
            {
                var caixilhosDaFamilia = await _caixilhoRepository.GetByFamiliaIdAsync(familia.IdFamiliaCaixilho);
                todosOsCaixilhos.AddRange(caixilhosDaFamilia);
            }

            ViewBag.Familias = familias;
            ViewBag.Caixilhos = todosOsCaixilhos;

            return View(obra);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> ConcluirObra(int id)
        {
            try
            {
                await _obraRepository.ConcluirAsync(id);
                TempData["SuccessMessage"] = "Obra concluída com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao concluir obra: {ex.Message}";
            }
            return RedirectToAction("Details", new { id });
        }

        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _obraRepository.GetById(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [Authorize(Roles = "Administrador,Gerente")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int idObra)
        {
            var obra = await _obraRepository.GetById(idObra);
            if (obra == null) return NotFound();

            RemoverImagemFisica(obra.ImagemObraPath);

            await _obraRepository.DeleteAsync(idObra);
            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> SalvarImagemAsync(IFormFile? arquivo)
        {
            if (arquivo == null || arquivo.Length == 0) return null;
            if (!ValidarImagem(arquivo, out _)) return null;

            var pastaDestino = Path.Combine(_webHostEnvironment.WebRootPath, "img");
            Directory.CreateDirectory(pastaDestino);

            var extensao = Path.GetExtension(arquivo.FileName);
            var nomeArquivo = $"obra_{Guid.NewGuid():N}{extensao}";
            var caminhoCompleto = Path.Combine(pastaDestino, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            return Path.Combine("img", nomeArquivo).Replace("\\", "/");
        }

        private void RemoverImagemFisica(string? caminhoRelativo)
        {
            if (string.IsNullOrWhiteSpace(caminhoRelativo)) return;

            var caminhoNormalizado = caminhoRelativo.Replace("/", Path.DirectorySeparatorChar.ToString());
            var caminhoCompleto = Path.Combine(_webHostEnvironment.WebRootPath, caminhoNormalizado);

            if (System.IO.File.Exists(caminhoCompleto))
            {
                System.IO.File.Delete(caminhoCompleto);
            }
        }

        private bool ValidarImagem(IFormFile arquivo, out string? mensagemErro)
        {
            mensagemErro = null;

            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (!_extensoesPermitidas.Contains(extensao))
            {
                mensagemErro = "Formato de imagem nao suportado. Utilize JPG, JPEG, PNG, GIF, BMP ou WEBP.";
                return false;
            }

            if (arquivo.Length > _tamanhoMaximoImagemBytes)
            {
                mensagemErro = "O arquivo excede o tamanho maximo permitido de 5 MB.";
                return false;
            }

            return true;
        }
    }
}
