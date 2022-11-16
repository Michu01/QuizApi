using System.Data.Common;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Enums;
using QuizApi.Extensions;
using QuizApi.Models;
using QuizApi.Services;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionSetsController : ControllerBase
    {
        private readonly QuizDbContext dbContext;

        private readonly ILogger<QuestionSetsController> logger;

        public QuestionSetsController(QuizDbContext dbContext, ILogger<QuestionSetsController> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        private async Task<QuestionSetDTO?> Find(int id)
        {
            return await dbContext.QuestionSets.FindAsync(id);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get(
            int pageId = 0, 
            int limit = 10, 
            string? namePattern = null, 
            int? categoryId = null)
        {
            limit = Math.Min(limit, 100);

            IEnumerable<QuestionSetDTO> questionSets = dbContext.QuestionSets
                .AsEnumerable()
                .Where(qs => User.CanAccess(qs));

            if (!string.IsNullOrEmpty(namePattern))
            {
                questionSets = questionSets.Where(s => s.Name.Contains(namePattern));
            }

            if (categoryId is not null)
            {
                questionSets = questionSets.Where(s => s.CategoryId == categoryId);
            }

            questionSets = questionSets.Skip(limit * pageId).Take(limit);

            return Ok(questionSets);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            if (await Find(id) is not QuestionSetDTO questionSetDTO)
            {
                return NotFound();
            }

            if (User.CanAccess(questionSetDTO))
            {
                return Ok(questionSetDTO);
            }

            return Forbid();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(QuestionSet questionSet)
        {
            try
            {
                int userId = User.GetId();

                QuestionSetDTO questionSetDTO = new()
                {
                    Name = questionSet.Name,
                    Access = questionSet.Access,
                    CategoryId = questionSet.CategoryId,
                    CreatorId = userId
                };

                await dbContext.QuestionSets.AddAsync(questionSetDTO);
                await dbContext.SaveChangesAsync();

                return Ok(questionSetDTO);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("{Message}", ex.InnerException?.Message);

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError("{Message}", ex.Message);

                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Patch(int id, QuestionSet questionSet)
        {
            if (await Find(id) is not QuestionSetDTO questionSetDTO)
            {
                return NotFound();
            }

            if (!User.CanModify(questionSetDTO))
            {
                return Forbid();
            }

            questionSetDTO.Name = questionSet.Name;
            questionSetDTO.Access = questionSet.Access;
            questionSetDTO.CategoryId = questionSet.CategoryId;

            await dbContext.SaveChangesAsync();

            return Ok(questionSetDTO);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (await Find(id) is not QuestionSetDTO questionSetDTO)
            {
                return NotFound();
            }

            if (!User.CanModify(questionSetDTO))
            {
                return Forbid();
            }

            dbContext.Remove(questionSetDTO);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}