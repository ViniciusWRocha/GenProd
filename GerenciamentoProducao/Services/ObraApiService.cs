using GerenciamentoProducao.ApiDtos;
using GerenciamentoProducao.Interfaces;
using GerenciamentoProducao.Models;

namespace GerenciamentoProducao.Services
{
    public class ObraApiService : ApiClientBase, IObraRepository
    {
        public ObraApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor) { }

        public async Task<List<Obra>> GetAllAsync()
        {
            var dtos = await GetListAsync<ObraResponseDto>("/api/obra");
            return dtos.Select(MapToModel).ToList();
        }

        public async Task<List<Obra>> GetAllFinalizadosAsync()
        {
            var dtos = await GetListAsync<ObraResponseDto>("/api/obra?finalizadas=true");
            return dtos.Select(MapToModel).ToList();
        }

        public async Task<List<Obra>> GetAllNaoFinalizadosAsync()
        {
            var dtos = await GetListAsync<ObraResponseDto>("/api/obra?finalizadas=false");
            return dtos.Select(MapToModel).ToList();
        }

        public async Task<Obra> GetById(int id)
        {
            var dto = await GetAsync<ObraResponseDto>($"/api/obra/{id}");
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
                DataInicio = ToJsonUtcDateTime(obra.DataInicio),
                DataTermino = ToJsonUtcDateTime(obra.DataTermino),
                PesoFinal = obra.PesoFinal,
                Observacoes = obra.Observacoes,
                IdUsuario = obra.IdUsuario
            };
            var response = await PostAsync<ObraCreateDto, ObraResponseDto>("/api/obra", dto);
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
                DataInicio = ToJsonUtcDateTime(obra.DataInicio),
                DataTermino = ToJsonUtcDateTime(obra.DataTermino),
                PesoFinal = obra.PesoFinal,
                Observacoes = obra.Observacoes,
                Finalizado = obra.Finalizado,
                ImagemObraPath = obra.ImagemObraPath,
                IdUsuario = obra.IdUsuario
            };
            await PutAsync($"/api/obra/{obra.IdObra}", dto);
        }

        public async Task DeleteAsync(int id)
        {
            await base.DeleteAsync($"/api/obra/{id}");
        }

        public async Task RecalcularProgressoAsync(int obraId)
        {
            await PostAsync("/api/dashboard/atualizar-progresso-obras");
        }

        public async Task ConcluirAsync(int obraId)
        {
            await PostAsync($"/api/obra/{obraId}/concluir");
        }

        // Datas Unspecified (ex.: input date) serializam sem 'Z'; a API espera instante UTC explícito.
        private static DateTime ToJsonUtcDateTime(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value.Date, DateTimeKind.Utc)
            };
        }

        private static string MapStatusObra(int status) => status switch
        {
            1 => "Cadastrada",
            2 => "Verificada",
            3 => "EmMedicao",
            4 => "EmProducao",
            5 => "Concluida",
            _ => "Cadastrada"
        };

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
                StatusObra = MapStatusObra(dto.StatusObra),
                PercentualMedicao = dto.PercentualMedicao,
                PercentualProducao = dto.PercentualProducao,
                Usuario = dto.NomeUsuario != null ? new Usuario { NomeUsuario = dto.NomeUsuario, IdUsuario = dto.IdUsuario } : null
            };
        }
    }
}
