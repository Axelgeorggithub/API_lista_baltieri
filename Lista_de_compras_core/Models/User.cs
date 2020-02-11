using System.ComponentModel.DataAnnotations; //tamanho de campo, chave
using System.ComponentModel.DataAnnotations.Schema;

namespace Lista_de_compras_core.Models
{
    [Table("Usuario")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatótio")]
        [Column("user_name")]
        [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatótio")]
        [Column("user_passw")]
        [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        public string Password { get; set; }

        [Column("user_tipo")]
        public string Role { get; set; }
    }
}
