namespace GerenciamentoProducao.ApiDtos
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public int? IdUsuario { get; set; }
        public string? NomeUsuario { get; set; }
        public string? Email { get; set; }
        public int? TipoUsuario { get; set; }
        public string? NomeTipoUsuario { get; set; }
        public string? Message { get; set; }
        public string? Cargo { get; set; }
        public List<string>? Cargos { get; set; }
        public string? Token { get; set; }
    }
}
