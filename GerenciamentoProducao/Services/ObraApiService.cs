using GerenciamentoProducao.ApiDtos;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Services
{
    public class ObraApiService : ApiClientBase, IObraRepository
    {
        public ObraApiService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<List<Obra>> GetAllAsync()
        {
            var dtos = await GetListAsync<ObraResponseDto>("/api/ObraApi");
            return dtos.Select(MapToModel).ToList();
        }

        public async Task<List<Obra>> GetAllFinalizadosAsync()
        {
            var dtos = await GetListAsync<ObraResponseDto>("/api/ObraApi?finalizadas=true");
            return dtos.Select(MapToModel).ToList();
        }

        public async Task<List<Obra>> GetAllNaoFinalizadosAsync()
        {
            var dtos = await GetListAsync<ObraResponseDto>("/api/ObraApi?finalizadas=false");
            return dtos.Select(MapToModel).ToList();
        }

        public async Task<Obra> GetById(int id)
        {
            var dto = await GetAsync<ObraResponseDto>($"/api/ObraApi/{id}");
            return dto != null ? MapToModel(dto) : throw new Exception("Obra não encontrada");
        }

        public async Task AddAsync(Obra obra)
        {
            var dto = new ObraCreateDto
            {
                Nome = obra.Nome,
                Construtora = obra.Construtora,
                Nro = obra.Nro,
                Logradouro = obra.Logradouro,
                Bairro = obra.Bairro,
                Cep = obra.Cep,
                Uf = obra.Uf,
                Cnpj = obra.Cnpj,
                DataInicio = obra.DataInicio,
                DataTermino = obra.DataTermino,
                PesoFinal = 0,
                Observacoes = obra.Observacoes,
                IdUsuario = obra.IdUsuario,
                IdResponsavelVerificacao = obra.IdResponsavelVerificacao,
                IdResponsavelMedicao = obra.IdResponsavelMedicao,
                IdResponsavelProducao = obra.IdResponsavelProducao
            };
            var response = await PostAsync<ObraCreateDto, ObraResponseDto>("/api/ObraApi", dto);
            if (response != null)
                obra.IdObra = response.IdObra;
        }

        public async Task UpdateAsync(Obra obra)
        {
            var dto = new ObraUpdateDto
            {
                Nome = obra.Nome,
                Construtora = obra.Construtora,
                Nro = obra.Nro,
                Logradouro = obra.Logradouro,
                Bairro = obra.Bairro,
                Cep = obra.Cep,
                Uf = obra.Uf,
                Cnpj = obra.Cnpj,
                DataInicio = obra.DataInicio,
                DataTermino = obra.DataTermino,
                PesoFinal = 0,
                Observacoes = obra.Observacoes,
                StatusObra = obra.StatusObra,
                Finalizado = obra.Finalizado,
                ImagemObraPath = obra.ImagemObraPath,
                IdUsuario = obra.IdUsuario,
                IdResponsavelVerificacao = obra.IdResponsavelVerificacao,
                IdResponsavelMedicao = obra.IdResponsavelMedicao,
                IdResponsavelProducao = obra.IdResponsavelProducao
            };
            await PutAsync($"/api/ObraApi/{obra.IdObra}", dto);
        }

        public async Task DeleteAsync(int id)
        {
            await base.DeleteAsync($"/api/ObraApi/{id}");
        }

        public async Task RecalcularProgressoAsync(int obraId)
        {
            await PostAsync($"/api/ObraApi/{obraId}/recalcular-progresso");
        }

        public async Task ConcluirAsync(int obraId)
        {
            await PostAsync($"/api/ObraApi/{obraId}/concluir");
        }

        private static Obra MapToModel(ObraResponseDto dto)
        {
            return new Obra
            {
                IdObra = dto.IdObra,
                Nome = dto.Nome,
                Construtora = dto.Construtora,
                Nro = dto.Nro,
                Logradouro = dto.Logradouro,
                Bairro = dto.Bairro,
                Cep = dto.Cep,
                Uf = dto.Uf,
                Cnpj = dto.Cnpj,
                DataInicio = dto.DataInicio,
                DataTermino = dto.DataTermino,
                PesoFinal = dto.PesoFinal,
                PesoProduzido = dto.PesoProduzido,
                PercentualConclusao = dto.PercentualConclusao,
                DataConclusao = dto.DataConclusao,
                Observacoes = dto.Observacoes,
                Finalizado = dto.Finalizado,
                ImagemObraPath = dto.ImagemObraPath,
                IdUsuario = dto.IdUsuario,
                StatusObra = dto.StatusObra,
                PercentualMedicao = dto.PercentualMedicao,
                PercentualProducao = dto.PercentualProducao,
                IdResponsavelVerificacao = dto.IdResponsavelVerificacao,
                IdResponsavelMedicao = dto.IdResponsavelMedicao,
                IdResponsavelProducao = dto.IdResponsavelProducao,
                NomeResponsavelVerificacao = dto.NomeResponsavelVerificacao,
                NomeResponsavelMedicao = dto.NomeResponsavelMedicao,
                NomeResponsavelProducao = dto.NomeResponsavelProducao,
                Usuario = dto.NomeUsuario != null ? new Usuario { NomeUsuario = dto.NomeUsuario, IdUsuario = dto.IdUsuario } : null
            };
        }
    }
}
