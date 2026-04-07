namespace API.SIGE.DTOs
{
    public class NotificacaoResponseDto
    {
        public int IdNotificacao { get; set; }
        public int IdUsuarioDestino { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public bool Lida { get; set; }
        public DateTime DataCriacao { get; set; }
        public string TipoNotificacao { get; set; } = string.Empty;
        public int? IdObra { get; set; }
        public string? NomeObra { get; set; }
    }
}
