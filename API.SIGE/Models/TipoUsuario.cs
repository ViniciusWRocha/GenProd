using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.SIGE.Models
{
    [Table("TipoUsuario")]
    public class TipoUsuario
    {
        [Key]
        public int IdTipoUsuario { get; set; }
        [Required(ErrorMessage = "Campo Obrigatório")]
        [StringLength(100)]
        public string NomeTipoUsuario { get; set; }

        //[Key]
        //public TipoUsuarioEnum TipoUsuarioEnum { get; set; }

        //public List<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }

    //public enum TipoUsuarioEnum
    //{
    //    Administrador = 1,
    //    Gerente = 2,
    //    Funcionario = 3
    //}
}
