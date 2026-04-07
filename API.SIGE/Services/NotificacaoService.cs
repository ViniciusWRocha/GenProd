using API.SIGE.DTOs;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Interfaces.Services;
using API.SIGE.Models;

namespace API.SIGE.Services
{
    public class NotificacaoService : INotificacaoService
    {
        private readonly INotificacaoRepository _notificacaoRepository;

        public NotificacaoService(INotificacaoRepository notificacaoRepository)
        {
            _notificacaoRepository = notificacaoRepository;
        }

        public async Task<List<NotificacaoResponseDto>> GetByUsuarioIdAsync(int idUsuario)
        {
            var notificacoes = await _notificacaoRepository.GetByUsuarioIdAsync(idUsuario);
            return notificacoes.Select(MapToDto).ToList();
        }

        public async Task<List<NotificacaoResponseDto>> GetNaoLidasAsync(int idUsuario)
        {
            var notificacoes = await _notificacaoRepository.GetNaoLidasAsync(idUsuario);
            return notificacoes.Select(MapToDto).ToList();
        }

        public async Task MarcarLidaAsync(int id)
        {
            var notificacao = await _notificacaoRepository.GetByIdAsync(id);
            if (notificacao == null)
                throw new InvalidOperationException($"Notificação com ID {id} não encontrada.");

            notificacao.Lida = true;
            await _notificacaoRepository.UpdateAsync(notificacao);
        }

        public async Task CriarAsync(int idDestino, string titulo, string mensagem, TipoNotificacao tipo, int? idObra = null)
        {
            var notificacao = new Notificacao
            {
                IdUsuarioDestino = idDestino,
                Titulo = titulo,
                Mensagem = mensagem,
                TipoNotificacao = tipo,
                IdObra = idObra,
                DataCriacao = DateTime.UtcNow,
                Lida = false
            };

            await _notificacaoRepository.AddAsync(notificacao);
        }

        private static NotificacaoResponseDto MapToDto(Notificacao n)
        {
            return new NotificacaoResponseDto
            {
                IdNotificacao = n.IdNotificacao,
                IdUsuarioDestino = n.IdUsuarioDestino,
                Titulo = n.Titulo,
                Mensagem = n.Mensagem,
                Lida = n.Lida,
                DataCriacao = n.DataCriacao,
                TipoNotificacao = n.TipoNotificacao.ToString(),
                IdObra = n.IdObra,
                NomeObra = n.Obra?.Nome
            };
        }
    }
}
