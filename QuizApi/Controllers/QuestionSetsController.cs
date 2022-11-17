using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Extensions;
using QuizApi.Models;

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

        [HttpGet("Mine")]
        [Authorize]
        public IActionResult GetMine()
        {
            int id = User.GetId();

            IQueryable<QuestionSetDTO> questionSets = dbContext.QuestionSets
                .Where(s => s.CreatorId == id);

            return Ok(questionSets);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([Required] QuestionSet questionSet)
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

                return CreatedAtAction(nameof(Post), questionSetDTO);
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
        public async Task<IActionResult> Patch(int id, [Required] QuestionSet questionSet)
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

        [HttpGet("{id:int}/Questions")]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestions(int id)
        {
            if (await Find(id) is not QuestionSetDTO questionSetDTO)
            {
                return NotFound();
            }

            if (!User.CanAccess(questionSetDTO))
            {
                return Forbid();
            }

            IQueryable<QuestionDTO> questions = dbContext.Questions
                .Where(q => q.QuestionSetId == id);

            return Ok(questions);
        }

        [HttpGet("{id:int}/Questions/{index}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestion(int id, int index)
        {
            if (await Find(id) is not QuestionSetDTO questionSetDTO)
            {
                return NotFound();
            }

            if (!User.CanAccess(questionSetDTO))
            {
                return Forbid();
            }

            IQueryable<QuestionDTO> questions = dbContext.Questions
                .Where(q => q.QuestionSetId == id);

            if (index < 0 || index >= questions.Count())
            {
                return NotFound();
            }

            QuestionDTO question = questions.ElementAt(index);

            return Ok(question);
        }

        [HttpPost("{id:int}/Questions")]
        [Authorize]
        public async Task<IActionResult> PostQuestion(int id, [Required] Question question)
        {
            if (await Find(id) is not QuestionSetDTO questionSetDTO)
            {
                return NotFound();
            }

            if (!User.CanModify(questionSetDTO))
            {
                return Forbid();
            }

            QuestionDTO questionDTO = new()
            {
                AnswerA = question.AnswerA,
                AnswerB = question.AnswerB,
                AnswerC = question.AnswerC,
                AnswerD = question.AnswerD,
                CorrectAnswer = question.CorrectAnswer,
                Contents = question.Contents,
                QuestionSetId = id
            };

            dbContext.Questions.Add(questionDTO);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuestion), questionDTO);
        }
    }
}