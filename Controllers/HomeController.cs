using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Backoffice.Controllers
{
    [Route("v1")]
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<dynamic>> Get([FromServices] DataContext context)
        {
            var employee = new User { Id = 1, Username = "robin", Password = "robin123"};
            var manager = new User { Id = 1, Username = "batman", Password = "batman123"};
            var category = new Category { Id = 1, Title = "Geral"};
            var product = new Product { Id = 1, Title = "iPhone", Description = "iPhone 12 Lançamento", Price = 12000, Category = category};
            context.Users.Add(employee);
            context.Users.Add(manager);
            context.Categories.Add(category);
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return Ok(new {message = "Configurado com sucesso"});
        }
    }
}