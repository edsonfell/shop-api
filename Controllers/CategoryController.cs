using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices] DataContext context)
        {
            try
            {
                //AsNoTracking() don't consider any other funcion that EF could
                //give to the object. That's ideal for read only, listing or things
                //that won't need any manipulation on the database; 
                return await context.Categories.AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível realizar sua solicitação" });
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(int id,
            [FromServices] DataContext context)
        {
            try
            {
                var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return NotFound(new { message = "Categoria não encontrada" });
                return category;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível realizar sua solicitação" });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles="employee, manager")]
        public async Task<ActionResult<Category>> Post(
            [FromBody] Category category,
            [FromServices] DataContext context)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                context.Categories.Add(category);
                await context.SaveChangesAsync();
                return Ok(category);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível realizar sua solicitação" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles="employee, manager")]
        public async Task<ActionResult<Category>> Put(int id,
        [FromBody] Category category,
        [FromServices] DataContext context)
        {
            try
            {
                if (category.Id != id)
                    return NotFound(new { message = "Categoria não encontrada" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // * In the bellow line we are defining the 'category' object as
                // 'modified'. It is enough to EF understant that we are updating
                // an existing register on the database and we just need to save it *
                context.Entry<Category>(category).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(category);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este registro acabou de ser atualizado" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível realizar sua solicitação" });
            }

        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles="employee, manager")]
        public async Task<ActionResult<Category>> Delete(int id,
        [FromServices] DataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return NotFound(new { message = "Categoria não encontrada" });
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível realizar sua solicitação" });
            }
        }
    }
}