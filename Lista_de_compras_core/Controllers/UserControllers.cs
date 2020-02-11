using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Lista_de_compras_core.Data;
using Lista_de_compras_core.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Lista_de_compras_core.Services;


namespace Lista_de_compras_core.Controllers
{
    [Route("v1/usuarios")]
    public class UserControllers : Controller
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "Gerente")]          //so o gerente pode ver os usuarios
        public async Task<ActionResult<List<User>>> Get(
            [FromServices] DataContext context)
        {
            var users = await context
                .Users
                .AsNoTracking()
                .ToListAsync();
            return users;
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        //[Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Post(
            [FromBody]User model,
            [FromServices]DataContext context)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                //Força o usuário a ser sempre funcionario, pq qualquer um pode ser um gerente e fazer o que quiser, durante a criação o role do usuario vai ser sempre Funcionario
                model.Role = "Funcionario";

                context.Users.Add(model);
                await context.SaveChangesAsync();

                //quando retorna o model esconde a senha
                model.Password = "";
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel criar o usuário" });
            }

        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authentication(        //esta dynamic pois pode tanto retornar um usuário quanto retornar null
            [FromBody] User model,
            [FromServices]DataContext context)
        {
            var user = await context.Users
                 .AsNoTracking()
                 .Where(x => x.Username == model.Username && x.Password == model.Password)
                 .FirstOrDefaultAsync();

            if (user == null)                                            //se não for encontrado nenhum usuario
            {
                return NotFound(new { message = "Usuário ou senha inválido" });
            }

            var token = TokenService.GenerateToken(user);               //gera o token
            model.Password = "";                                        //esconde a senha
            return new                                                  //retornando um novo objeto
            {
                user = user,
                token = token
            };
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult<User>> Put(
            [FromBody] User model,
            [FromServices] DataContext context,
            int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(id != model.Id)
            {
                return NotFound(new { message = "Usuario não encontrado" });
            }
            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel criar o usuário" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Gerente")]
        public async Task<ActionResult<User>> Delete(
            [FromServices]DataContext context,
            int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "Usuario não encontrado" });
            }
            try
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return Ok(new { message = "Usuario removido com sucesso" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possivel remover o usuario" });
            }
        }
    }
}
