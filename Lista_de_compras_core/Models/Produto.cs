using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lista_de_compras_core.Models
{
    [Table("Product")]//nome da tabela
    public class Produto
    {
        [Key]
        [Column("prod_id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatótio")] //not null
        [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        [Column("prod_title")] // nome da coluna
        public string Titulo { get; set; }

        [MaxLength(1024, ErrorMessage = "Esse campo deve conter no máximo 1024 caracteres")]
        [Column("prod_desc")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatótio")]
        [Range(1, int.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        [Column("prod_price")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatótio")]
        [Range(1, int.MaxValue, ErrorMessage = "Categoria inválida")]
        [Column("prod_cat_id")]
        public int CategoriaId { get; set; }

        public Categoria Categoria { get; set; }
    }
}
