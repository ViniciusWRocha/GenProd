using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.SIGE.Models
{
    [Table ("Caixilho")]
    public class Caixilho
    {
        [Key]
        public int IdCaixilho { get; set; }
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome do Caixilho deve ter entre 3 e 100 caracteres")]
        public string NomeCaixilho { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Largura { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Altura { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int Quantidade{ get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public float PesoUnitario { get; set; }

        public bool Liberado { get; set; } = false;

        public DateTime? DataLiberacao { get; set; }

        [StringLength(200)]
        public string? Observacoes { get; set; }

        public string? DescricaoCaixilho { get; set; }


        public StatusProducao StatusProducao { get; set; } // enum PARA MEDIÇÃO - PRODUÇÃO - CONCLUIDO

        [ForeignKey("Obra")]
        public int ObraId { get; set; }
        [JsonIgnore]
        public virtual Obra? Obra{ get; set; }

        [ForeignKey("FamiliaCaixilho")]
        public int IdFamiliaCaixilho { get; set; }
        [JsonIgnore]
        public virtual FamiliaCaixilho? FamiliaCaixilho { get; set; }
        
    }

    public enum StatusProducao
    {
        ParaMedir = 1,
        Medido = 2,
        Concluido = 3,
        Pendente = 4
    }

}
