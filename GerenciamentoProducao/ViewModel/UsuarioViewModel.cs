using GerenciamentoProducao.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GerenciamentoProducaoo.ViewModel;

public class UsuarioViewModel
{
    public int IdUsuario { get; set; }
    [Required(ErrorMessage = "Informe o nome do usuário")]
    [StringLength(50, ErrorMessage = "O nome deve ter até 50 caracteres")]
    public string NomeUsuario { get; set; }

    [Required(ErrorMessage = "Informe o e-mail")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
    [StringLength(100, ErrorMessage = "O e-mail deve ter até 100 caracteres")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Informe a senha")]
    [StringLength(6, MinimumLength = 4, ErrorMessage = "A senha deve conter até 6 caracteres")]
    public string Senha { get; set; }

    [Required(ErrorMessage = "Informe o telefone")]
    [StringLength(14, ErrorMessage = "O telefone deve ter até 14 caracteres")]
    public string Telefone { get; set; }
    public int IdTipoUsuario { get; set; }
    public bool Ativo { get; set; } = true;

    //colecao para popular o dropdownlist
    public IEnumerable<SelectListItem>? TiposUsuario { get; set; }

}
