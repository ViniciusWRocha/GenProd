namespace API.SIGE.DTOs
{
    public class UsuarioResponseDto
    {
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public int IdTipoUsuario { get; set; }
        public string? NomeTipoUsuario { get; set; }
        public List<CargoResponseDto> Cargos { get; set; } = new();
    }
}
