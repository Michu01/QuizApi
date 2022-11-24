using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Models;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly QuizDbContext dbContext;

        public CategoriesController(QuizDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(dbContext.QuestionSetCategories);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            if (await dbContext.QuestionSetCategories.FindAsync(id) is not CategoryDTO questionSetCategory)
            {
                return NotFound();
            }

            return Ok(questionSetCategory);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([Required] Category questionSetCategory)
        {
            CategoryDTO dto = new()
            {
                Name = questionSetCategory.Name,
            };

            dbContext.QuestionSetCategories.Add(dto);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Post), dto);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await dbContext.QuestionSetCategories.FindAsync(id) is not CategoryDTO dto)
            {
                return NotFound();
            }

            dbContext.QuestionSetCategories.Remove(dto);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}