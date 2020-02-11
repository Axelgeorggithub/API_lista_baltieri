using Lista_de_compras_core.Data;
using Lista_de_compras_core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lista_de_compras_core.Controllers
{
    //https://localhost:5001/v1/categorias
    //http://localhost:5000/v1/categorias

    [Route("v1/categorias")]
    public class CategoriaControllers : ControllerBase
    {
        [HttpGet]
        [Route("")]                                      // esta sem rota, mas se quiser pode-se colocar assim [Route("get")], para dessa forma evitar de fazer codigo desnecessario para verificação
        [AllowAnonymous]
        //ou eu coloco services.AddResponseCaching(); -> no Startup
        //ou eu especifico o cache como esse de baixo
        [ResponseCache(VaryByHeader = "User-Agent", Location =ResponseCacheLocation.Any, Duration =30)]
        //se eu colocar esse services.AddResponseCaching(); posso dizer quais não terão cache assim 
        //[ResponseCache(Duration =0,Location =ResponseCacheLocation.None, NoStore ==true)]
        public async Task<ActionResult<List<Categoria>>> Get(
            [FromServices]DataContext context
            )                                                   //o task, trabalha de forma assíncrona, de forma paralela, desse modo não travando a trad principal
        {                                                       // colocar o async na frente para mostrar que esse metodo é assíncrono, ???nao é sempre???
                                                                //public async Task<IActionResult> Get()  ou   public async Task<ActionResult<Categoria>> Get() 

            var categorias = await context.Categorias.AsNoTracking().ToListAsync();          //.AsNoTracking()  ->  ele faz uma leitura do banco da forma mais rapida possivel e joga para a tela, se for SÓ leitura usar essa, caso eu precise usar em outro local não usar. ele tbm so pega os dados necessarios 
                                                                                             //.ToListAsync()  -> ele executa essa query de fato, sempre no final, pois,  caso eu tenha que fazer um order by, where ... fazer antes, caso eu coloque um filtro dps desse metodo, ele vai fazer o filtro em memoria que vai dar problema  
            return Ok(categorias);
        }

        [HttpGet]
        [Route("{id:int}")]                             // o que estiver entre chaves será um parametro, e para colocar uma restrição para rota, colocar os dois pontos após o parametro, como por exemplo, [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Categoria>> GetById(
            int id,
            [FromServices]DataContext context)
        {
            var categorias = await context.Categorias.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return categorias;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "Funcionario")]
        public async Task<ActionResult<List<Categoria>>> Post(
            [FromBody]Categoria model,                  //[FromBody] vai vir uma categoria no corpo da riquisição, nessa linha ele tenta ler um model no corpo da requisição
            [FromServices]DataContext context           //[FromServices] vai vir uma datacontext do serviso 
        )
        {
            if (!ModelState.IsValid)                    //é preciso ver se o model retornado é valido de acordo com as suas retrições descritas na sua classe
            {                                            //ModelState <- tem o estado do modelo passado 
                return BadRequest(ModelState);
            }
            try                                             // é colocado o try caso haj um erro na inserção no banco
                    {
                    context.Categorias.Add(model);          //se eu fizer  context.Categorias.Add(model); ele vai adicionar no bd em memória mas ñão vai persistir em lugar nenhum, para persistir é necessario usar uma função chamada savechange
                    await context.SaveChangesAsync();       //desse modo estou salvando de uma forma assincrona, o await espera o add salvar no banco primeiro
                                                            //no momento que é dado o savechange, ja é gerado um ID automatico e ja preenche o meu model
                     return Ok(model);                      //return await TESTE(); so para mostrar como seria retornar um metodo async
                }
                catch (Exception)
                {
                     return BadRequest(new { message = "Não foi possivel criar"});
                }
                                                            //nao retorna um model, por isso usar o ok(), para mostrar que deu certo
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "Funcionario")]
        public async Task<ActionResult<List<Categoria>>> Put(
            int id, 
            [FromBody]Categoria model,
            [FromServices]DataContext context)          //public Categoria Put(int id, [FromBody]Categoria model) <- estava assim, mas pode ser assim ->  public ActionResult Put(int id, [FromBody]Categoria model)---------((ActionResult)desse modo fica masi formal e ja faz parte do ControllerBase, ja trazendo um resultado como a tela espera
        {
            //verifica se o ID informado é o mesmo do modelo
            if (model.Id != id)
            {
                return NotFound(new { message = "Categoria não encontrada" });
            }
            //verifica se os dados são validos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

                try                                         //para fazer a atualização de uma entidade 
                {  //as entradas no contexto de uma categoria(tipo da entrada) do modelo, assim fica, o estado do modelo fica como modificado
                    context.Entry<Categoria>(model).State = EntityState.Modified;
                    await context.SaveChangesAsync();                   //context. qualque coisa ele faz em MEMORIA, ou seja, não esta persistido no banco ainda, por isso é necessario usar o savechange para persistir 
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
        [Route("{id:int}")]
        [Authorize(Roles = "Funcionario")]
        public async Task<ActionResult<List<Categoria>>> Delete(
            int id,
            [FromServices]DataContext context)
        {
            //aqui a baixo eu recupero a categoria para remover la em baixo
            var category = await context.Categorias.FirstOrDefaultAsync(x => x.Id == id); //busca uma categoria, se ele achar mais de uma ele pega a primeira, se não achar nada ele retorna null, se achar os uma ele pega essa  
                                                                                          //na função (x => x.Id == id) x é uma categoria, se o id da categoria for igual ao id passado por parametro  
            if (category == null)
            {
                return NotFound(new { message = "Categoria não encontrada" });
            }
            try
            {
                context.Categorias.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new { message ="Categoria removida com sucesso"});              //ou poderia tbm so colocar assim -> return Ok(category);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel remover a categoria" });
            }
        }
    }
}
