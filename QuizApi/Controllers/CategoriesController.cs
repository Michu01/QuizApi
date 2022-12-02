using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Models;
using QuizApi.Repositories;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesRepository repository;

        public CategoriesController(ICategoriesRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<CategoryDTO>> Get()
        {
            IEnumerable<CategoryDTO> categories = repository.Get();

            return Ok(categories);
        }
        
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDTO>> Get(int id)
        {
            if (await repository.Find(id) is not CategoryDTO category)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CategoryDTO>> Post([Required] Category category)
        {
            CategoryDTO categoryDTO = new()
            {
                Name = category.Name
            };

            repository.Add(categoryDTO);
            await repository.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = categoryDTO.Id }, categoryDTO);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (await repository.Find(id) is not CategoryDTO category)
            {
                return NotFound();
            }

            repository.Remove(category);
            await repository.SaveChangesAsync();

            return Ok();
        }
    }
}