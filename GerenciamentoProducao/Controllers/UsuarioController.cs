using System.Security.Claims;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using GerenciamentoProducaoo.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GerenciamentoProducao.Controllers;

public class UsuarioController : Controller
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITipoUsuarioRepository _tipoUsuarioRepository;

    public UsuarioController(
        IUsuarioRepository usuarioRepository,
        ITipoUsuarioRepository tipoUsuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
        _tipoUsuarioRepository = tipoUsuarioRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl)
    {
        if (User?.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Usuario");
        }
        ViewData["ReturnUrl"] = returnUrl;
        return View("Login");
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string email, string senha)
    {
        var usuario = await _usuarioRepository.ValidarLoginAsync(email, senha);

        if (usuario == null || !usuario.Ativo)
        {
            ModelState.AddModelError(string.Empty, "Usuario ou senha invalidos");
            return View("Login");
        }

        string role = NormalizeRole(usuario?.TipoUsuario?.NomeTipoUsuario);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.NomeUsuario ?? usuario.Email ?? "Usuario"),
            new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, role),
            new Claim("IdUsuario", usuario.IdUsuario.ToString())
        };

        var identity = new ClaimsIdentity(claims, "GerenciadorProd");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("GerenciadorProd", principal);

        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("GerenciadorProd");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AcessoNegado()
    {
        return View();
    }

    public async Task<IActionResult> Index(int? tipoUsuarioId, string? search)
    {
        var usuarios = await _usuarioRepository.GetAllAtivosAsync();

        if (tipoUsuarioId.HasValue && tipoUsuarioId.Value > 0)
            usuarios = usuarios.Where(u => u.IdTipoUsuario == tipoUsuarioId).ToList();

        if (!string.IsNullOrEmpty(search))
            usuarios = usuarios.Where(u => u.NomeUsuario.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        usuarios = usuarios.OrderByDescending(u => u.IdUsuario).ToList();

        ViewBag.TiposUsuario = new SelectList(await _tipoUsuarioRepository.GetAllAsync(), "IdTipoUsuario", "NomeTipoUsuario");
        ViewBag.FiltroTipoId = tipoUsuarioId;
        ViewBag.TermoBusca = search;

        return View(usuarios);
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,Gerente")]
    public async Task<IActionResult> Create()
    {
        var vm = await CriarUsuarioViewModel();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UsuarioViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var vm = await CriarUsuarioViewModel(viewModel);
            return View(vm);
        }

        var usuario = new Usuario
        {
            NomeUsuario = viewModel.NomeUsuario,
            Email = viewModel.Email,
            Senha = viewModel.Senha,
            Telefone = viewModel.Telefone,
            IdTipoUsuario = viewModel.IdTipoUsuario,
            Ativo = true
        };

        await _usuarioRepository.AddAsync(usuario);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,Gerente")]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0) return NotFound();

        var usuario = await _usuarioRepository.GetById(id);
        if (usuario == null) return NotFound();

        var vm = new UsuarioViewModel
        {
            IdUsuario = usuario.IdUsuario,
            NomeUsuario = usuario.NomeUsuario,
            Email = usuario.Email,
            Senha = usuario.Senha,
            Telefone = usuario.Telefone,
            IdTipoUsuario = usuario.IdTipoUsuario,
            TiposUsuario = (await _tipoUsuarioRepository.GetAllAsync()).Select(t => new SelectListItem
            {
                Value = t.IdTipoUsuario.ToString(),
                Text = t.NomeTipoUsuario
            })
        };

        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador,Gerente")]
    public async Task<IActionResult> Edit(int id, UsuarioViewModel viewModel)
    {
        if (id != viewModel.IdUsuario) return NotFound();

        if (!ModelState.IsValid)
        {
            viewModel = await CriarUsuarioViewModel(viewModel);
            return View(viewModel);
        }

        var usuario = await _usuarioRepository.GetById(id);
        if (usuario == null) return NotFound();

        usuario.NomeUsuario = viewModel.NomeUsuario;
        usuario.Email = viewModel.Email;
        usuario.Senha = viewModel.Senha;
        usuario.Telefone = viewModel.Telefone;
        usuario.IdTipoUsuario = viewModel.IdTipoUsuario;

        await _usuarioRepository.UpdateAsync(usuario);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0) return NotFound();

        var usuario = await _usuarioRepository.GetById(id);
        if (usuario == null) return NotFound();

        var vm = new UsuarioViewModel
        {
            IdUsuario = usuario.IdUsuario,
            NomeUsuario = usuario.NomeUsuario,
            Email = usuario.Email,
            Senha = usuario.Senha,
            Telefone = usuario.Telefone,
            IdTipoUsuario = usuario.IdTipoUsuario,
            Ativo = usuario.Ativo
        };

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _usuarioRepository.InativarAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Inativos()
    {
        var usuarios = await _usuarioRepository.GetAllInativosAsync();
        return View(usuarios);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var usuario = await _usuarioRepository.GetById(id.Value);
        if (usuario == null) return NotFound();

        return View(usuario);
    }

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Ativar(int id)
    {
        if (id <= 0) return NotFound();

        await _usuarioRepository.ReativarAsync(id);

        return RedirectToAction(nameof(Inativos));
    }

    private async Task<UsuarioViewModel> CriarUsuarioViewModel(UsuarioViewModel? model = null)
    {
        var tipos = await _tipoUsuarioRepository.GetAllAsync();
        return new UsuarioViewModel
        {
            IdUsuario = model?.IdUsuario ?? 0,
            NomeUsuario = model?.NomeUsuario,
            Email = model?.Email,
            Senha = model?.Senha,
            Telefone = model?.Telefone,
            IdTipoUsuario = model?.IdTipoUsuario ?? 0,
            Ativo = model?.Ativo ?? true,
            TiposUsuario = tipos.Select(t => new SelectListItem
            {
                Value = t.IdTipoUsuario.ToString(),
                Text = t.NomeTipoUsuario
            })
        };
    }

    private static string NormalizeRole(string? raw)
    {
        var r = (raw ?? string.Empty).Trim().ToLowerInvariant();
        return r switch
        {
            "administrador" or "admin" => "Administrador",
            "gerente" or "maneger" => "Gerente",
            _ => "Outros"
        };
    }
}
