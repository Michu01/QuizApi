using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using Moq;

using NuGet.ContentModel;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Enums;
using QuizApi.Models;
using QuizApi.Repositories;
using QuizApi.Services;

namespace QuizApiTests
{
    public class QuizesRepositoryTests
    {
        private static Mock<QuizDbContext> CreateDbContext(IQueryable<QuizDTO> query)
        {
            Mock<DbSet<QuizDTO>> dbSet = new();

            dbSet.As<IQueryable<QuizDTO>>().Setup(q => q.ElementType).Returns(query.ElementType);
            dbSet.As<IQueryable<QuizDTO>>().Setup(q => q.Expression).Returns(query.Expression);
            dbSet.As<IQueryable<QuizDTO>>().Setup(q => q.Provider).Returns(query.Provider);
            dbSet.As<IQueryable<QuizDTO>>().Setup(q => q.GetEnumerator()).Returns(() => query.GetEnumerator());

            Mock<QuizDbContext> dbContext = new();
            dbContext.SetupGet(x => x.Quizes).Returns(dbSet.Object);

            return dbContext;
        }

        [Fact]
        public async Task Find_QuizExists_ShouldReturnQuiz()
        {
            int id = 0;

            QuizDTO expected = new() { Id = id };

            Mock<QuizDbContext> dbContext = new();
            dbContext.Setup(d => d.Quizes.FindAsync(It.IsAny<int>())).ReturnsAsync((QuizDTO?)null);
            dbContext.Setup(d => d.Quizes.FindAsync(id)).ReturnsAsync(expected);

            Mock<IAuthorizationService> authorizationService = new();

            IQuizesRepository quizesRepository = new QuizesRepository(dbContext.Object, authorizationService.Object);

            QuizDTO? actual = await quizesRepository.Find(id);

            authorizationService.VerifyNoOtherCalls();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Get_ShouldReturnMatching()
        {
            IList<QuizDTO> quizes = new List<QuizDTO>()
            {
                new() { CategoryId = 0, CreatorId = 0, Name = "Test" },
                new() { CategoryId = 0, CreatorId = 0, Name = "Tst" },
                new() { CategoryId = 1, CreatorId = 0, Name = "Test" },
                new() { CategoryId = 0, CreatorId = 1, Name = "Test" }
            };

            IList<QuizDTO> expected = new List<QuizDTO>()
            {
                quizes[0]
            };

            Mock<QuizDbContext> dbContext = CreateDbContext(quizes.AsQueryable());
                
            Mock<IAuthorizationService> authorizationService = new();
            authorizationService.Setup(a => a.CanUserAccessQuiz(It.IsAny<ClaimsPrincipal>(), It.IsAny<QuizDTO>())).ReturnsAsync(true);

            IQuizesRepository quizesRepository = new QuizesRepository(dbContext.Object, authorizationService.Object);

            IEnumerable<QuizDTO> actual = quizesRepository
                .Get(0, 10, "tes", 0, 0, null, new ClaimsPrincipal())
                .ToEnumerable();

            dbContext.VerifyGet(d => d.Quizes, Times.Once());

            Assert.Equal(expected, actual);
        }
    }
}
