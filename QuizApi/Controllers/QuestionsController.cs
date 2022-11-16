using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class QuestionsController : ControllerBase
    {
        private readonly QuizDbContext dbContext;

        public QuestionsController(QuizDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private async Task<QuestionDTO?> Find(int id)
        {
            return await dbContext.Questions.FindAsync(id);
        }

        private async Task<QuestionSetDTO?> FindQuestionSet(int id)
        {
            return await dbContext.QuestionSets.FindAsync(id);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int questionSetId)
        {
            if (await FindQuestionSet(questionSetId) is not QuestionSetDTO questionSetDTO)
            {
                return NotFound();
            }

            if (!User.CanAccess(questionSetDTO))
            {
                return Forbid();
            }

            await dbContext.Entry(questionSetDTO).Collection(q => q.Questions).LoadAsync();

            return Ok(questionSetDTO.Questions);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(int questionSetId, Question question)
        {
            if (await FindQuestionSet(questionSetId) is not QuestionSetDTO questionSetDTO)
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
                QuestionSetId = questionSetId
            };

            dbContext.Questions.Add(questionDTO);
            await dbContext.SaveChangesAsync();

            return Ok(questionDTO);
        }

        [HttpPatch("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Patch(int id, Question question)
        {
            if (await Find(id) is not QuestionDTO questionDTO)
            {
                return NotFound();
            }

            if (!User.CanModify(questionDTO.QuestionSet))
            {
                return Forbid();
            }

            questionDTO.Contents = question.Contents;
            questionDTO.AnswerA = question.AnswerA;
            questionDTO.AnswerB = question.AnswerB;
            questionDTO.AnswerC = question.AnswerC;
            questionDTO.AnswerD = question.AnswerD;
            questionDTO.CorrectAnswer = question.CorrectAnswer;

            await dbContext.SaveChangesAsync();

            return Ok(questionDTO);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (await Find(id) is not QuestionDTO questionDTO)
            {
                return NotFound();
            }

            if (!User.CanModify(questionDTO.QuestionSet))
            {
                return Forbid();
            }

            dbContext.Questions.Remove(questionDTO);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
