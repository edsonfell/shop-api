using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Controllers
{
    [Route("products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get(
            [FromServices] DataContext context)
        {
            try
            {
                //The "Include" will bring all the Category info related to the
                //product we are searching 
                return await context.Products
                .Include(x => x.Category).AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível realizar sua solicitação" });
            }
        }
        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(int id,
            [FromServices] DataContext context)
        {
            try
            {
                var product = await context.Products
                .Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (product == null)
                    return NotFound(new { message = "Produto não encontrado" });
                return product;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível realizar sua solicitação" });
            }
        }
        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(int id,   
            [FromServices] DataContext context)
        {
            try
            {
                return await context.Products
                .Include(x => x.Category)
                .AsNoTracking()
                .Where(x => x.CategoryId == id)
                .ToListAsync();
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível realizar sua solicitação" });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles="employee, manager")]
        public async Task<ActionResult<Product>> Post(
            [FromBody] Product product,
            [FromServices] DataContext context)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                context.Products.Add(product);
                await context.SaveChangesAsync();
                return Ok(product);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível realizar sua solicitação" });
            }
        }
    }
}