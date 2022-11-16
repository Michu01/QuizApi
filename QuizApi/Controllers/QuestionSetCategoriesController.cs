using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Models;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionSetCategoriesController : ControllerBase
    {
        private readonly QuizDbContext dbContext;

        public QuestionSetCategoriesController(QuizDbContext dbContext)
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
            if (await dbContext.QuestionSetCategories.FindAsync(id) is not QuestionSetCategoryDTO questionSetCategory)
            {
                return NotFound();
            }

            return Ok(questionSetCategory);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(QuestionSetCategory questionSetCategory)
        {
            QuestionSetCategoryDTO dto = new()
            {
                Name = questionSetCategory.Name,
            };

            dbContext.QuestionSetCategories.Add(dto);
            await dbContext.SaveChangesAsync();

            return Ok(dto);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await dbContext.QuestionSetCategories.FindAsync(id) is not QuestionSetCategoryDTO dto)
            {
                return NotFound();
            }

            dbContext.QuestionSetCategories.Remove(dto);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}