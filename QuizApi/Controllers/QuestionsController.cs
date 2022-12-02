using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.DTOs;

using QuizApi.Models;
using QuizApi.Repositories;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionsRepository repository;

        private readonly Services.IAuthorizationService authorizationService;

        private readonly IQuizesRepository quizesRepository;

        public QuestionsController(
            IQuestionsRepository repository,
            Services.IAuthorizationService authorizationService, 
            IQuizesRepository quizesRepository)
        {
            this.repository = repository;
            this.authorizationService = authorizationService;
            this.quizesRepository = quizesRepository;
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionDTO>> Get(int id)
        {
            if (await repository.Find(id) is not QuestionDTO questionDTO)
            {
                return NotFound();
            }

            QuizDTO quiz = (await quizesRepository.Find(id))!;

            if (!await authorizationService.CanUserAccessQuiz(User, quiz))
            {
                return Forbid();
            }

            return Ok(questionDTO);
        }

        [HttpPatch("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionDTO>> Patch(int id, [Required] Question question)
        {
            if (await repository.Find(id) is not QuestionDTO questionDTO)
            {
                return NotFound();
            }

            QuizDTO quiz = (await quizesRepository.Find(id))!;

            if (!authorizationService.CanUserModifyQuiz(User, quiz))
            {
                return Forbid();
            }

            questionDTO.Contents = question.Contents;
            questionDTO.AnswerA = question.AnswerA;
            questionDTO.AnswerB = question.AnswerB;
            questionDTO.AnswerC = question.AnswerC;
            questionDTO.AnswerD = question.AnswerD;
            questionDTO.CorrectAnswer = question.CorrectAnswer;

            await repository.SaveChangesAsync();

            return Ok(questionDTO);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (await repository.Find(id) is not QuestionDTO questionDTO)
            {
                return NotFound();
            }

            QuizDTO quiz = (await quizesRepository.Find(id))!;

            if (!authorizationService.CanUserModifyQuiz(User, quiz))
            {
                return Forbid();
            }

            repository.Remove(questionDTO);
            await repository.SaveChangesAsync();

            return Ok();
        }
    }
}