using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoProducao.Controllers

{
    public class TipoUsuarioController : Controller
    {
        private readonly ITipoUsuarioRepository _tipoUsuarioRepository;
        public TipoUsuarioController(ITipoUsuarioRepository tipoUsuarioRepository)
        {
            _tipoUsuarioRepository = tipoUsuarioRepository;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _tipoUsuarioRepository.GetAllAsync();
            return View(lista);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Gerente")]
        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(TipoUsuario tipoUsuario)
        {
            if (ModelState.IsValid)
            {
                await _tipoUsuarioRepository.AddAsync(tipoUsuario);
                return RedirectToAction(nameof(Index));
            }
            return View(tipoUsuario);
        }
        [HttpGet]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Edit(int id)
        {
            var tipoUsuario = await _tipoUsuarioRepository.GetById(id);
            if (tipoUsuario == null) return NotFound();
            return View(tipoUsuario);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoUsuario tipoUsuario)
        {
            if (id != tipoUsuario.IdTipoUsuario)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _tipoUsuarioRepository.UpdateAsync(tipoUsuario);
                return RedirectToAction(nameof(Index));
            }
            return View(tipoUsuario);
        }

        //[HttpGet]
        [Authorize(Roles = "Administrador,Gerente")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _tipoUsuarioRepository.GetById(id);
            if (item == null) return NotFound();
            return View(item); ;
        }
        //autorize aqui tbm

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _tipoUsuarioRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
