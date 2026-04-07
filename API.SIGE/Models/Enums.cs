namespace API.SIGE.Models
{
    public enum StatusObra
    {
        Cadastrada = 1,
        Verificada = 2,
        EmMedicao = 3,
        EmProducao = 4,
        Concluida = 5
    }

    public enum StatusFamilia
    {
        Pendente = 1,
        EmMedicao = 2,
        Medida = 3,
        EmProducao = 4,
        Produzida = 5
    }

    public enum StatusAtividade
    {
        NaoIniciada = 1,
        EmAndamento = 2,
        Pausada = 3,
        Concluida = 4
    }

    public enum TipoCargo
    {
        Gerente = 1,
        ResponsavelVerificacao = 2,
        ResponsavelMedicao = 3,
        ResponsavelProducao = 4
    }

    public enum TipoAnexo
    {
        Medicao = 1,
        Producao = 2
    }

    public enum TipoNotificacao
    {
        ObraVerificada = 1,
        FamiliaMedida = 2,
        FamiliaProduzida = 3,
        ObraConcluida = 4
    }
}
