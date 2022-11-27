using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Enums;
using QuizApi.Extensions;
using QuizApi.Models;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizesController : ControllerBase
    {
        private readonly QuizDbContext dbContext;

        private readonly ILogger<QuizesController> logger;

        public QuizesController(QuizDbContext dbContext, ILogger<QuizesController> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        private async Task<QuizDTO?> Find(int id)
        {
            return await dbContext.QuestionSets.FindAsync(id);
        }

        private async Task<bool> IsNameConflict(string name)
        {
            return await dbContext.QuestionSets.AnyAsync(q => q.Name == name);
        }

        private async Task<bool> IsNameConflict(string name, int id)
        {
            return await dbContext.QuestionSets.AnyAsync(q => q.Id != id && q.Name == name);
        }

        private async Task<bool> DoesCategoryExist(int categoryId)
        {
            return await dbContext.QuestionSetCategories.FindAsync(categoryId) is not null;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get(
            int pageId = 0, 
            int limit = 10, 
            string? namePattern = null,
            int? categoryId = null,
            int? creatorId = null,
            CreatorFilter? creatorFilter = null)
        {
            limit = Math.Min(limit, 100);

            IQueryable<QuizDTO> questionSetsQuery = dbContext.QuestionSets;

            if (!string.IsNullOrEmpty(namePattern))
            {
                questionSetsQuery = questionSetsQuery.Where(s => s.Name.Contains(namePattern));
            }

            if (categoryId is not null)
            {
                questionSetsQuery = questionSetsQuery.Where(s => s.CategoryId == categoryId);
            }

            if (creatorId is not null)
            {
                questionSetsQuery = questionSetsQuery.Where(s => s.CreatorId == creatorId);
            }

            IEnumerable<QuizDTO> questionSets = questionSetsQuery;

            if (creatorFilter is not null)
            {
                if (User.Identity is null)
                {
                    return BadRequest("User not signed in");
                }

                int id = User.GetId();

                if (creatorFilter == CreatorFilter.Me)
                {
                    questionSets = questionSets.Where(s => s.CreatorId == id);
                }
                else if (creatorFilter == CreatorFilter.Friends)
                {
                    IEnumerable<QuizDTO> friendsQuestionSets = dbContext.GetUserFriendsQuestionSets(id);

                    questionSets = questionSets.Intersect(friendsQuestionSets);
                }
                else throw new NotImplementedException();
            }

            IAsyncEnumerable<QuizDTO> questionSetsAsync = questionSets
                .ToArray()
                .ToAsyncEnumerable()
                .WhereAwait(async qs => await User.CanAccess(qs, dbContext));

            questionSetsAsync = questionSetsAsync.Skip(limit * pageId).Take(limit);

            return Ok(questionSetsAsync);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            if (await Find(id) is not QuizDTO questionSetDTO)
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
        public async Task<IActionResult> Post([Required] Quiz questionSet)
        {
            if (await IsNameConflict(questionSet.Name))
            {
                return Conflict($"Quiz with name \"{questionSet.Name}\" already exists");
            }

            if (!await DoesCategoryExist(questionSet.CategoryId))
            {
                return NotFound($"No category with id: {questionSet.CategoryId} found");
            }

            int userId = User.GetId();

            QuizDTO questionSetDTO = new()
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
        public async Task<IActionResult> Patch(int id, [Required] Quiz questionSet)
        {
            if (await Find(id) is not QuizDTO questionSetDTO)
            {
                return NotFound();
            }

            if (!User.CanModify(questionSetDTO))
            {
                return Forbid();
            }

            if (await IsNameConflict(questionSet.Name, id))
            {
                return Conflict($"Quiz with name \"{questionSet.Name}\" already exists");
            }

            if (!await DoesCategoryExist(questionSet.CategoryId))
            {
                return NotFound($"No category with id: {questionSet.CategoryId} found");
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
            if (await Find(id) is not QuizDTO questionSetDTO)
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
            if (await Find(id) is not QuizDTO questionSetDTO)
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
            if (await Find(id) is not QuizDTO questionSetDTO)
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
            if (await Find(id) is not QuizDTO questionSetDTO)
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