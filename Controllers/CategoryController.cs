using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Controllers
{

    [ApiController]
    public class CategoryController : ControllerBase
    {

        [HttpGet("categories")]//sempre no minusculo e no plural
        [HttpGet("categorias")]
        [HttpGet("v2/categorias")]
        public async Task<IActionResult> GetAsyncCache(
          [FromServices] IMemoryCache cache,
          [FromServices] BlogDataContext context)
        {
            try
            {
                var categorias = cache.GetOrCreate("CategoriesCache", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return GetCategories(context);
                });

                //   await context.Categories.ToListAsync();
                return Ok(new ResultViewModel<List<Category>>(categorias));
                // return Ok(categorias);
            }
            catch
            {

                return StatusCode(500, new ResultViewModel<List<Category>>("05X04 - Falha interna no servidor"));
            }

        }

        private List<Category> GetCategories(BlogDataContext context) => context.Categories.ToList();

        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context)
        {
            try
            {
                var categories = await context.Categories.ToListAsync();
                return Ok(categories);
            }
            catch
            {
                return StatusCode(500, "05X04 - Falha interna no servidor");
            }
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
         [FromRoute] int id, [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    NotFound(("Couteúdo não encontrado."));
                return Ok(category);
            }
            catch
            {
                return StatusCode(500, "Falha interna no servidor");
            }

        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync(
         [FromRoute] int id,
         [FromBody] EditorCategoryViewModel model,
         [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

                category.Name = model.Name;
                category.Slug = model.Slug;
                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05x11 - Não foi possivel incluir a categoria ex:" + ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05x12 - falha interna no servidor: ex:" + ex.Message));
            }
        }


        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync([FromBody] EditorCategoryViewModel model, [FromServices] BlogDataContext context)
        {

            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Category>("Conteúdo não encontrado."));
            try
            {

                var category = new Category
                {
                    Id = 0,
                    Posts = null,
                    Name = model.Name,
                    Slug = model.Slug.ToLower(),
                };

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();
                return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("0XE9 - Não foi possivel incluir a categoria"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05x10 -Falha interna no servidor"));
            }
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync( [FromRoute] int id, [FromServices] BlogDataContext context)
        {
       
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return BadRequest(new ResultViewModel<Category>("Conteúdo não encontrado."));

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", category);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "05x13 - Não foi possivel deletar a categoria");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "05x14 - falha interna no servidor");

            }
        }


    }
}
