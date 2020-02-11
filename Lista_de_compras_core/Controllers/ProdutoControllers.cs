using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lista_de_compras_core.Models;
using Lista_de_compras_core.Data;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Lista_de_compras_core.Controllers
{
    //https://localhost:5001/produtos
    //http://localhost:5000/produtos

    [Route("v1/produtos")] //versionamento
    public class ProdutoControllers : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Produto>>> Get(
           [FromServices]DataContext context)                                                  
        {                                                      
            var produto = await context.Produtos
                .Include(x => x.Categoria)                                  //inclui as categorias quando eu buscar os produtos. Da um join no sql server. pode ter mais de um include
                .AsNoTracking().ToListAsync();

            return produto;
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Produto>> GetById(
           int id,
           [FromServices]DataContext context)
        {
            var produto = await context.Produtos
                .Include(x => x.Categoria)
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            return produto;
        }

        [HttpGet]
        [Route("categorias/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Produto>>> GetByCategory(
           int id,
           [FromServices]DataContext context)
        {
            var produtos = await context.Produtos
                .Include(x => x.Categoria)
                .AsNoTracking().Where(x => x.CategoriaId == id)         //quero listar todos os produtos de acordo com o id de uma categoria
                .ToListAsync();

            return produtos;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "Funcionario")]
        public async Task<ActionResult<Produto>> Post(
          [FromServices]DataContext context,
          [FromBody]Produto model)
        {
            if (ModelState.IsValid)
            {
                context.Produtos.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut]
        [Route("")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult<Produto>> Put(
            int id,
            [FromBody]Produto model,
            [FromServices]DataContext context)
        {
            if(model.Id != id)
            {
                return NotFound(new { message = "Produto não encotrado" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Entry<Produto>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este registro ja foi atualizado" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel criar" });
            }
        }

        [HttpDelete]
        [Route("")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult<Produto>> Delete(
            int id,
            [FromServices]DataContext context)
        {
            var produto = await context.Produtos.FirstOrDefaultAsync(x => x.Id == id);
        
            if(produto == null)
            {
                return NotFound(new { message = "Produto não encotrado" });
            }
            try
            {
                context.Produtos.Remove(produto);
                await context.SaveChangesAsync();
                return Ok(new { message = "Produto removida com sucesso" });              //ou poderia tbm so colocar assim -> return Ok(category);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel remover o produto" });
            }
        }
    }
}
