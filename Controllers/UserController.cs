using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;

namespace Controllers
{
    [Route("users")]
    public class UserController : Controller
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles="manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
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
        public async Task<ActionResult<User>> Post (
            [FromServices] DataContext context,
            [FromBody] User user)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                user.Role = "employee";
                context.Users.Add(user);
                await context.SaveChangesAsync();
                return Ok(user.Username);
            }
            catch
            {
                return BadRequest(new {message = "Erro ao inserir usuário"});
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles="manager")]
        public async Task<ActionResult<User>> Put (
            [FromServices] DataContext context,
            int id,
            [FromBody] User user)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if(id != user.Id)
                return NotFound(new {message = "Usuário não encontrado"});

            try
            {
                context.Entry(user).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(user);
            }
            catch
            {
                return BadRequest(new {message = "Erro ao inserir usuário"});
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate (
            [FromServices] DataContext context,
            [FromBody] User model)
        {
            var user = await context.Users
            .AsNoTracking()
            .Where(x => x.Username == model.Username && x.Password == model.Password)
            .FirstOrDefaultAsync();

            if(user == null)
                return NotFound(new {message = "Usuário ou senha inválidos"});
            
            var token = TokenService.GenerateToken(user);
            return new 
            {
                user = user.Username,
                token = token
            };
        }
    }
}