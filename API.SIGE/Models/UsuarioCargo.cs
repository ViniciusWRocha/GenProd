using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.SIGE.Models
{
    [Table("UsuarioCargo")]
    public class UsuarioCargo
    {
        [Key]
        public int IdUsuarioCargo { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [JsonIgnore]
        public virtual Usuario? Usuario { get; set; }

        [Required]
        [ForeignKey("Cargo")]
        public int IdCargo { get; set; }

        [JsonIgnore]
        public virtual Cargo? Cargo { get; set; }
    }
}
