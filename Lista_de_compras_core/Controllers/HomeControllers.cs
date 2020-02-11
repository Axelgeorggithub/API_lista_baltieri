using Lista_de_compras_core.Data;
using Lista_de_compras_core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lista_de_compras_core.Controllers
{
    [Route("v1")]
    public class HomeControllers : Controller
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<dynamic>> Get(
            [FromServices]DataContext context)
        {
            var funcionario = new User { Id = 1, Username = "robin", Password = "robin", Role = "Funcionario" };
            var gerente = new User { Id = 2, Username = "batman", Password = "batman", Role = "Gerente" };
            var categoria = new Categoria { Id = 1, Titulo = "Informática" };
            var produto = new Produto { Id = 1, Categoria = categoria, Titulo = "Mouse", Preco = 299, Descricao = "Mouse Gamer" };
            context.Users.Add(funcionario);
            context.Users.Add(gerente);
            context.Categorias.Add(categoria);
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            return Ok(new { message = "Dados configurados"});
        }   
    }
}
