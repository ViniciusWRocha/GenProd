namespace GerenciamentoProducao.ApiDtos
{
    public class CargoResponseDto
    {
        public int IdCargo { get; set; }
        public string TipoCargo { get; set; } = string.Empty;
        public string DescricaoCargo { get; set; } = string.Empty;
    }
}
