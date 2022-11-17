using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.DbContexts;
using QuizApi.DTOs;

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

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            if (await Find(id) is not QuestionDTO questionDTO)
            {
                return NotFound();
            }

            if (!User.CanAccess(questionDTO.QuestionSet))
            {
                return Forbid();
            }

            return Ok(questionDTO);
        }

        [HttpPatch("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Patch(int id, [Required] Question question)
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
