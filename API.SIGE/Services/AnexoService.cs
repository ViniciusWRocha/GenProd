using API.SIGE.DTOs;
using API.SIGE.Interfaces.Repositories;
using API.SIGE.Interfaces.Services;
using API.SIGE.Models;
using Microsoft.Extensions.Configuration;

namespace API.SIGE.Services
{
    public class AnexoService : IAnexoService
    {
        private readonly IAnexoRepository _anexoRepository;
        private readonly IConfiguration _configuration;

        public AnexoService(IAnexoRepository anexoRepository, IConfiguration configuration)
        {
            _anexoRepository = anexoRepository;
            _configuration = configuration;
        }

        public async Task<AnexoResponseDto> UploadAsync(AnexoUploadDto dto)
        {
            var basePath = _configuration["FileStorage:BasePath"] ?? "C:/SIGE/Uploads";
            var maxSizeMB = int.Parse(_configuration["FileStorage:MaxFileSizeMB"] ?? "10");
            var allowedExtensions = (_configuration["FileStorage:AllowedExtensions"] ?? ".pdf,.jpg,.jpeg,.png,.xlsx,.docx")
                .Split(',', StringSplitOptions.RemoveEmptyEntries);

            var extension = Path.GetExtension(dto.Arquivo.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException($"Extensão '{extension}' não permitida.");

            if (dto.Arquivo.Length > maxSizeMB * 1024 * 1024)
                throw new InvalidOperationException($"Arquivo excede o tamanho máximo de {maxSizeMB}MB.");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(basePath, fileName);

            Directory.CreateDirectory(basePath);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Arquivo.CopyToAsync(stream);
            }

            // Ler bytes do arquivo para armazenar no banco
            byte[] dados;
            using (var memoryStream = new MemoryStream())
            {
                await dto.Arquivo.CopyToAsync(memoryStream);
                dados = memoryStream.ToArray();
            }

            var anexo = new Anexo
            {
                NomeArquivo = dto.Arquivo.FileName,
                CaminhoArquivo = filePath,
                TipoArquivo = dto.Arquivo.ContentType,
                TamanhoBytes = dto.Arquivo.Length,
                Dados = dados,
                DataUpload = DateTime.UtcNow,
                TipoAnexo = (TipoAnexo)dto.TipoAnexo,
                IdMedicao = dto.IdMedicao,
                IdProducaoFamilia = dto.IdProducaoFamilia,
                IdUsuario = dto.IdUsuario
            };

            await _anexoRepository.AddAsync(anexo);

            var result = await _anexoRepository.GetByIdAsync(anexo.IdAnexo);
            return MapToDto(result!);
        }

        public async Task<List<AnexoResponseDto>> GetByMedicaoIdAsync(int medicaoId)
        {
            var anexos = await _anexoRepository.GetByMedicaoIdAsync(medicaoId);
            return anexos.Select(MapToDto).ToList();
        }

        public async Task<List<AnexoResponseDto>> GetByProducaoFamiliaIdAsync(int producaoFamiliaId)
        {
            var anexos = await _anexoRepository.GetByProducaoFamiliaIdAsync(producaoFamiliaId);
            return anexos.Select(MapToDto).ToList();
        }

        public async Task DeleteAsync(int id)
        {
            var anexo = await _anexoRepository.GetByIdAsync(id);
            if (anexo == null)
                throw new InvalidOperationException($"Anexo com ID {id} não encontrado.");

            // Remover arquivo físico
            if (File.Exists(anexo.CaminhoArquivo))
            {
                File.Delete(anexo.CaminhoArquivo);
            }

            await _anexoRepository.DeleteAsync(id);
        }

        private static AnexoResponseDto MapToDto(Anexo a)
        {
            return new AnexoResponseDto
            {
                IdAnexo = a.IdAnexo,
                NomeArquivo = a.NomeArquivo,
                CaminhoArquivo = a.CaminhoArquivo,
                TipoArquivo = a.TipoArquivo,
                TamanhoBytes = a.TamanhoBytes,
                DataUpload = a.DataUpload,
                TipoAnexo = a.TipoAnexo.ToString(),
                IdMedicao = a.IdMedicao,
                IdProducaoFamilia = a.IdProducaoFamilia,
                IdUsuario = a.IdUsuario,
                NomeUsuario = a.Usuario?.NomeUsuario
            };
        }
    }
}
