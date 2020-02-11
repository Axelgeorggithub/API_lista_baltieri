using System.ComponentModel.DataAnnotations; //tamanho de campo, chave
using System.ComponentModel.DataAnnotations.Schema; //referente a todos pos esquemas do sql server, nome de campo/tabela, tipo de dado(usando o [DataType(nvarchar)])

namespace Lista_de_compras_core.Models
{
    [Table("Category")]
    public class Categoria
    {
        //DataAnnotations, são as anotações em cima
        [Key] //Primary key - AutoIncrement 
        [Column("cat_id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatótio")] //not null
        [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")] //esses erros vao aparecer se por um acaso
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        [Column("cat_title")]
        public string Titulo { get; set; }
    }
}
