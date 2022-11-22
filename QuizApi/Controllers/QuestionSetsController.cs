using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Enums;
using QuizApi.Extensions;
using QuizApi.Models;

using System.Collections.Concurrent;

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
            int? categoryId = null,
            QuestionSetCreatorFilter? creatorFilter = null)
        {
            limit = Math.Min(limit, 100);

            IAsyncEnumerable<QuestionSetDTO> questionSets = dbContext.QuestionSets
                .AsAsyncEnumerable()
                .WhereAwait(async qs => await User.CanAccess(qs, dbContext));

            if (!string.IsNullOrEmpty(namePattern))
            {
                questionSets = questionSets.Where(s => s.Name.Contains(namePattern));
            }

            if (categoryId is not null)
            {
                questionSets = questionSets.Where(s => s.CategoryId == categoryId);
            }

            if (creatorFilter is not null)
            {
                if (User.Identity is null)
                {
                    return BadRequest("User not signed in");
                }

                int id = User.GetId();

                if (creatorFilter == QuestionSetCreatorFilter.Me)
                {
                    questionSets = questionSets.Where(s => s.CreatorId == id);
                }
                else if (creatorFilter == QuestionSetCreatorFilter.Friends)
                {
                    IEnumerable<QuestionSetDTO> friendsQuestionSets = dbContext.GetUserFriendsQuestionSets(id);

                    questionSets = questionSets.Intersect(friendsQuestionSets.ToAsyncEnumerable());
                }
                else throw new NotImplementedException();
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

            if (await User.CanAccess(questionSetDTO, dbContext))
            {
                return Ok(questionSetDTO);
            }

            return Forbid();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([Required] QuestionSet questionSet)
        {
            int userId = User.GetId();

            QuestionSetDTO questionSetDTO = new()
            {
                Name = questionSet.Name,
                Description = questionSet.Description,
                Access = questionSet.Access,
                CategoryId = questionSet.CategoryId,
                CreatorId = userId
            };

            await dbContext.QuestionSets.AddAsync(questionSetDTO);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Post), questionSetDTO);
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
            questionSetDTO.Description = questionSet.Description;
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

            if (!await User.CanAccess(questionSetDTO, dbContext))
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

            if (!await User.CanAccess(questionSetDTO, dbContext))
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