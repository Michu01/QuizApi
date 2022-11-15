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

        [HttpGet]
        public IActionResult Get(
            int pageId = 0, 
            int limit = 10, 
            string? namePattern = null, 
            int? categoryId = null)
        {
            limit = Math.Min(limit, 100);

            IQueryable<QuestionSetDTO> questionSets = dbContext.QuestionSets;

            if (User.Identity is null)
            {
                questionSets = questionSets.Where(qs => qs.Access == QuestionSetAccess.Public);
            }
            else if (User.GetRole() != UserRole.Admin)
            {
                int userId = User.GetId();

                questionSets = questionSets.Where(qs => qs.Access != QuestionSetAccess.Private || qs.CreatorId == userId);
                questionSets = questionSets.Where(qs => qs.Access != QuestionSetAccess.Friends || qs.Creator.IsFriend(userId));
            }

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
        public async Task<IActionResult> Get(int id)
        {
            if (await Find(id) is not QuestionSetDTO questionSet)
            {
                return NotFound();
            }

            if (questionSet.Access == QuestionSetAccess.Public)
            {
                return Ok(questionSet);
            }

            if (User.Identity is null)
            {
                return Forbid();
            }

            if (User.GetRole() == UserRole.Admin)
            {
                return Ok(questionSet);
            }

            int userId = User.GetId();

            if (questionSet.Access == QuestionSetAccess.Private)
            {
                if (userId == questionSet.CreatorId)
                {
                    return Ok(questionSet);
                }

                return Forbid();
            }

            if (questionSet.Access == QuestionSetAccess.Friends)
            {
                if (questionSet.Creator.IsFriend(userId))
                {
                    return Ok(questionSet);
                }

                return Forbid();
            }

            throw new NotImplementedException();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(QuestionSet questionSet)
        {
            try
            {
                int userId = User.GetId();

                QuestionSetDTO dto = new()
                {
                    Name = questionSet.Name,
                    Access = questionSet.Access,
                    CategoryId = questionSet.CategoryId,
                    CreatorId = userId
                };

                await dbContext.QuestionSets.AddAsync(dto);
                await dbContext.SaveChangesAsync();

                return Ok(dto);
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
            if (await Find(id) is not QuestionSetDTO dto)
            {
                return NotFound();
            }

            if (User.GetRole() != UserRole.Admin && User.GetId() != dto.CreatorId)
            {
                return Forbid();
            }

            dto.Name = questionSet.Name;
            dto.Access = questionSet.Access;
            dto.CategoryId = questionSet.CategoryId;

            await dbContext.SaveChangesAsync();

            return Ok(dto);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (await Find(id) is not QuestionSetDTO dto)
            {
                return NotFound();
            }

            if (User.GetRole() != UserRole.Admin && User.GetId() != dto.CreatorId)
            {
                return Forbid();
            }

            dbContext.Remove(dto);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        private async Task<QuestionSetDTO?> Find(int id)
        {
            return await dbContext.QuestionSets.FindAsync(id);
        }
    }
}