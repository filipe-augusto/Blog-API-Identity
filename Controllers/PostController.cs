using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Text.RegularExpressions;

namespace Blog.Controllers
{
[ApiController]
    public class PostController : Controller
    {
        [HttpGet("v1/posts")]
        public async Task<IActionResult>  GetAsync(
            [FromServices] BlogDataContext context
            )
        {

            return Ok(await context.Posts.ToListAsync());
            //return View();
        }

        [HttpGet("v2/posts")]
        public async Task<IActionResult> GetAsync2(
          [FromServices] BlogDataContext context
          )
        {
            var posts = await context.Posts
                .AsTracking()
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Select(x => new
                {
                    x.Id, x.Title, x.Category.Name, x.Author.Email
                }).ToListAsync();
            return Ok(posts);
            //return View();
        }

        [HttpGet("v3/posts")]
        public async Task<IActionResult> GetAsync3(
       [FromServices] BlogDataContext context
       )
        {
            var posts = await context.Posts
                .AsTracking()
                .Include(x => x.Category)
                .Include(x => x.Author)
                 .Select(x => new ListPostsViewModel
                 {
                     Id = x.Id,
                     Title = x.Title,
                     Slug = x.Slug,
                     LastUpdateDate = x.LastUpdateDate,
                     Category = x.Category.Name,
                     Author = $"{x.Author.Name} ({x.Author.Email})"
                 })
                .ToListAsync();
            return Ok(posts);
            //return View();
        }
        [HttpGet("v4/posts")]
        public async Task<IActionResult> GetAsync4(//[FromQuery] vem da query
  [FromServices] BlogDataContext context,
                     [FromQuery] int page = 0,
                      [FromQuery] int pageSize = 25
                      )
        {

            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();
                var posts = await context.Posts
                    .AsTracking()
                    .Include(x => x.Category)
                    .Include(x => x.Author)
                     .Select(x => new ListPostsViewModel
                     {
                         Id = x.Id,
                         Title = x.Title,
                         Slug = x.Slug,
                         LastUpdateDate = x.LastUpdateDate,
                         Category = x.Category.Name,
                         Author = $"{x.Author.Name} ({x.Author.Email})"
                     })
                     .Skip(page * pageSize)
                     .Take(pageSize)
                    .ToListAsync();
                return Ok(new ResultViewModel<dynamic>(
                    new
                    {
                        total = count,
                        page,
                        pageSize,
                        posts
                    }));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05X04 - Falha interna no servidor"));
            }
            //return View();
        }

        [HttpGet("v5/posts/{id:int}")]
        public async Task<IActionResult> GetAsync5(//[FromQuery] vem da query
[FromServices] BlogDataContext context,
   [FromRoute] int id
              )
        {

            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();
                var post = await context.Posts
                    .AsTracking()
                    .Include(x => x.Author)
                    .ThenInclude( x => x.Roles)
                    .Include(x => x.Category)
                    .FirstOrDefaultAsync( x => x.Id == id);

                if (post == null)
                    return NotFound(new ResultViewModel<Post>("Conteudo não encontrado."));

                return Ok(new ResultViewModel<Post>(post));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05X04 - Falha interna no servidor"));
            }
            //return View();
        }

        [HttpGet("v1/posts/category/{category}")]
        public async Task<IActionResult> GetByCategoryAsync(
    [FromRoute] string category,
    [FromServices] BlogDataContext context,
    [FromQuery] int page = 0,
    [FromQuery] int pageSize = 25)
        {
            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();
                var posts = await context
                    .Posts
                    .AsNoTracking()
                    .Include(x => x.Author)
                    .Include(x => x.Category)
                    .Where(x => x.Category.Slug == category)
                    .Select(x => new ListPostsViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})"
                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(x => x.LastUpdateDate)
                    .ToListAsync();
                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05X04 - Falha interna no servidor"));
            }
        }

    }
}

    
