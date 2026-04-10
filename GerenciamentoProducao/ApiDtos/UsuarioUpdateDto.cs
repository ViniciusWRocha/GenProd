namespace GerenciamentoProducao.ApiDtos
{
    public class UsuarioUpdateDto
    {
        public string NomeUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Senha { get; set; }
        public string? Telefone { get; set; }
        public int IdTipoUsuario { get; set; }
    }
}
