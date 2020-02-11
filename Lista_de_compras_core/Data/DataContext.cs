using Lista_de_compras_core.Models;
using Microsoft.EntityFrameworkCore;

namespace Lista_de_compras_core.Data
{
    public class DataContext : DbContext
    //DbContext é uma representação do nosso banco em memória
    {
        public DataContext(DbContextOptions<DataContext> options)
            :base(options)
        {}
        public DbSet<Produto> Produtos { get; set; }    //busca uma tabela Produto
        public DbSet<Categoria> Categorias { get; set; }    //busca uma tabela Categoria
        public DbSet<User> Users { get; set; }      //busca uma tabela User
    }
}
