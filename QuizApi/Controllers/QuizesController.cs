using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.DTOs;
using QuizApi.Enums;
using QuizApi.Extensions;
using QuizApi.Models;
using QuizApi.Repositories;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizesController : ControllerBase
    {
        private const int MaxQuizLimit = 100;

        private readonly IQuizesRepository repository;

        private readonly Services.IAuthorizationService authorizationService;

        private readonly IQuestionsRepository questionsRepository;

        public QuizesController(
            IQuizesRepository repository, 
            Services.IAuthorizationService authorizationService, 
            IQuestionsRepository questionsRepository)
        {
            this.repository = repository;
            this.authorizationService = authorizationService;
            this.questionsRepository = questionsRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IAsyncEnumerable<QuizDTO>> Get(
            int pageId = 0, 
            int limit = 10, 
            string? namePattern = null,
            int? categoryId = null,
            int? creatorId = null,
            CreatorFilter? creatorFilter = null)
        {
            if (creatorFilter is not null && User.Identity is null)
            {
                return Unauthorized();
            }

            limit = Math.Min(limit, MaxQuizLimit);

            IAsyncEnumerable<QuizDTO> quizes = repository.Get(pageId, limit, namePattern, categoryId, creatorId, creatorFilter, User);

            return Ok(quizes);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuizDTO>> Get(int id)
        {
            if (await repository.Find(id) is not QuizDTO quiz)
            {
                return NotFound();
            }

            if (!await authorizationService.CanUserAccessQuiz(User, quiz))
            {
                return Forbid();
            }

            return Ok(quiz);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<QuizDTO>> Post([Required] Quiz quiz)
        {
            if (await repository.IsNameConflict(quiz.Name))
            {
                return Conflict($"Quiz with name \"{quiz.Name}\" already exists");
            }

            int userId = User.GetId();

            QuizDTO quizDTO = new()
            {
                Name = quiz.Name,
                Description = quiz.Description,
                Access = quiz.Access,
                CategoryId = quiz.CategoryId,
                CreatorId = userId
            };

            repository.Add(quizDTO);
            await repository.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = quizDTO.Id }, quizDTO);
        }

        [HttpPatch("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<QuizDTO>> Patch(int id, [Required] Quiz quiz)
        {
            if (await repository.Find(id) is not QuizDTO quizDTO)
            {
                return NotFound();
            }

            if (!authorizationService.CanUserModifyQuiz(User, quizDTO))
            {
                return Forbid();
            }

            if (quiz.Name != quizDTO.Name && await repository.IsNameConflict(quiz.Name))
            {
                return Conflict($"Quiz with name \"{quiz.Name}\" already exists");
            }

            quizDTO.Name = quiz.Name;
            quizDTO.Description = quiz.Description;
            quizDTO.Access = quiz.Access;
            quizDTO.CategoryId = quiz.CategoryId;
            
            await repository.SaveChangesAsync();

            return Ok(quizDTO);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (await repository.Find(id) is not QuizDTO quizDTO)
            {
                return NotFound();
            }

            if (!authorizationService.CanUserModifyQuiz(User, quizDTO))
            {
                return Forbid();
            }

            repository.Remove(quizDTO);
            await repository.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{quizId:int}/Questions")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<QuestionDTO>>> GetQuestions(int quizId)
        {
            if (await repository.Find(quizId) is not QuizDTO quizDTO)
            {
                return NotFound();
            }

            if (!await authorizationService.CanUserAccessQuiz(User, quizDTO))
            {
                return Forbid();
            }

            IEnumerable<QuestionDTO> questions = questionsRepository.FindByQuizId(quizId);

            return Ok(questions);
        }

        [HttpGet("{quizId:int}/Questions/{index}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionDTO>> GetQuestion(int quizId, int index)
        {
            if (await repository.Find(quizId) is not QuizDTO quizDTO)
            {
                return NotFound();
            }

            if (!await authorizationService.CanUserAccessQuiz(User, quizDTO))
            {
                return Forbid();
            }

            IEnumerable<QuestionDTO> questions = questionsRepository.FindByQuizId(quizId);

            if (index < 0 || index >= questions.Count())
            {
                return NotFound();
            }

            QuestionDTO question = questions.ElementAt(index);

            return Ok(question);
        }

        [HttpPost("{quizId:int}/Questions")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QuestionDTO>> PostQuestion(int quizId, [Required] Question question)
        {
            if (await repository.Find(quizId) is not QuizDTO quizDTO)
            {
                return NotFound();
            }

            if (!await authorizationService.CanUserAccessQuiz(User, quizDTO))
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
                QuizId = quizId
            };

            questionsRepository.Add(questionDTO);
            await questionsRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(QuestionsController.Get), new { id = questionDTO.Id }, questionDTO);
        }
    }
}