
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Moq;

using QuizApi.DTOs;
using QuizApi.Enums;
using QuizApi.Extensions;
using QuizApi.Repositories;

namespace QuizApiTests
{
    public class ClaimsPrincipalExtensionsTests
    {
        private static ClaimsPrincipal CreateClaimsPrincipal(string? id = null, string? role = null)
        {
            ClaimsIdentity identity = new();

            if (id is not null)
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
            }

            if (role is not null)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return new(identity);
        }

        [Fact]
        public void GetId_HasValidId_ShouldReturnId()
        {
            int id = 0;

            ClaimsPrincipal user = CreateClaimsPrincipal(id.ToString());

            int actual = user.GetId();

            Assert.Equal(id, actual);
        }

        [Fact]
        public void GetId_HasInvalidId_ShouldThrow()
        {
            ClaimsPrincipal user = CreateClaimsPrincipal("");

            Assert.ThrowsAny<Exception>(() => user.GetId());
        }

        [Fact]
        public void GetId_HasNoId_ShouldThrow()
        {
            ClaimsPrincipal user = CreateClaimsPrincipal();

            Assert.ThrowsAny<Exception>(() => user.GetId());
        }

        [Fact]
        public void TryGetId_HasValidId_ShouldReturnId()
        {
            int id = 0;

            ClaimsPrincipal user = CreateClaimsPrincipal(id.ToString());

            int? actual = user.TryGetId();

            Assert.Equal(id, actual);
        }

        [Fact]
        public void TryGetId_HasInvalidId_ShouldReturnNull()
        {
            ClaimsPrincipal user = CreateClaimsPrincipal("");

            int? actual = user.TryGetId();

            Assert.Null(actual);
        }

        [Fact]
        public void TryGetId_HasNoId_ShouldReturnNull()
        {
            ClaimsPrincipal user = CreateClaimsPrincipal(null);

            int? actual = user.TryGetId();

            Assert.Null(actual);
        }

        [Fact]
        public void TryGetRole_HasValidRole_ShouldReturnRole()
        {
            Role expected = Role.User;

            ClaimsPrincipal user = CreateClaimsPrincipal(role: expected.ToString());

            Role? actual = user.TryGetRole();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TryGetRole_HasInvalidRole_ShouldReturnNull()
        {
            ClaimsPrincipal user = CreateClaimsPrincipal(role: "");

            Role? actual = user.TryGetRole();

            Assert.Null(actual);
        }

        [Fact]
        public void TryGetRole_HasNoRole_ShouldReturnNull()
        {
            ClaimsPrincipal user = CreateClaimsPrincipal();

            Role? actual = user.TryGetRole();

            Assert.Null(actual);
        }

        [Fact]
        public async Task CanAccess_PublicQuiz_ShouldReturnTrue()
        {
            ClaimsPrincipal user = CreateClaimsPrincipal();

            QuizDTO quiz = new() { Access = Access.Public };

            Mock<IFriendshipsRepository> friendshipsRepository = new();

            bool actual = await user.CanAccess(quiz, friendshipsRepository.Object);

            friendshipsRepository.VerifyNoOtherCalls();
            Assert.True(actual);
        }

        [Theory]
        [InlineData(Access.Public)]
        [InlineData(Access.Private)]
        [InlineData(Access.Friends)]
        public async Task CanAccess_Admin_ShouldReturnTrue(Access access)
        {
            ClaimsPrincipal user = CreateClaimsPrincipal(role: Role.Admin.ToString());

            QuizDTO quiz = new() { Access = access };

            Mock<IFriendshipsRepository> friendshipsRepository = new();

            bool actual = await user.CanAccess(quiz, friendshipsRepository.Object);

            friendshipsRepository.VerifyNoOtherCalls();
            Assert.True(actual);
        }

        [Theory]
        [InlineData(Access.Private)]
        [InlineData(Access.Friends)]
        public async Task CanAccess_Anonymous_ShouldReturnFalse(Access access)
        {
            ClaimsPrincipal user = CreateClaimsPrincipal();

            QuizDTO quiz = new() { Access = access };

            Mock<IFriendshipsRepository> friendshipsRepository = new();

            bool actual = await user.CanAccess(quiz, friendshipsRepository.Object);

            friendshipsRepository.VerifyNoOtherCalls();
            Assert.False(actual);
        }

        [Fact]
        public async Task CanAccess_PrivateQuizWhenCreator_ShouldReturnTrue()
        {
            int creatorId = 0;

            ClaimsPrincipal user = CreateClaimsPrincipal(id: creatorId.ToString(), role: Role.User.ToString());

            QuizDTO quiz = new() { Access = Access.Private, CreatorId = creatorId };

            Mock<IFriendshipsRepository> friendshipsRepository = new();

            bool actual = await user.CanAccess(quiz, friendshipsRepository.Object);

            friendshipsRepository.VerifyNoOtherCalls();
            Assert.True(actual);
        }

        [Fact]
        public async Task CanAccess_PrivateQuizWhenNotCreator_ShouldReturnFalse()
        {
            ClaimsPrincipal user = CreateClaimsPrincipal(id: 0.ToString(), role: Role.User.ToString());

            QuizDTO quiz = new() { Access = Access.Private, CreatorId = 1 };

            Mock<IFriendshipsRepository> friendshipsRepository = new();

            bool actual = await user.CanAccess(quiz, friendshipsRepository.Object);

            friendshipsRepository.VerifyNoOtherCalls();
            Assert.False(actual);
        }

        [Fact]
        public async Task CanAccess_FriendsQuizWhenNotFriend_ShouldReturnFalse()
        {
            int userId = 0;
            int creatorId = 1;

            ClaimsPrincipal user = CreateClaimsPrincipal(id: userId.ToString(), role: Role.User.ToString());

            QuizDTO quiz = new() { Access = Access.Friends, CreatorId = creatorId };

            Mock<IFriendshipsRepository> friendshipsRepository = new();
            friendshipsRepository.Setup(fr => fr.AreUsersFriends(userId, creatorId)).Returns(Task.FromResult(false));

            bool actual = await user.CanAccess(quiz, friendshipsRepository.Object);

            friendshipsRepository.Verify(fr => fr.AreUsersFriends(userId, creatorId), Times.Once());
            friendshipsRepository.VerifyNoOtherCalls();
            Assert.False(actual);
        }

        [Fact]
        public async Task CanAccess_FriendsQuizWhenFriend_ShouldReturnTrue()
        {
            int userId = 0;
            int creatorId = 1;

            ClaimsPrincipal user = CreateClaimsPrincipal(id: userId.ToString(), role: Role.User.ToString());

            QuizDTO quiz = new() { Access = Access.Friends, CreatorId = creatorId };

            Mock<IFriendshipsRepository> friendshipsRepository = new();
            friendshipsRepository.Setup(fr => fr.AreUsersFriends(userId, creatorId)).Returns(Task.FromResult(true));

            bool actual = await user.CanAccess(quiz, friendshipsRepository.Object);

            friendshipsRepository.Verify(fr => fr.AreUsersFriends(userId, creatorId), Times.Once());
            friendshipsRepository.VerifyNoOtherCalls();
            Assert.True(actual);
        }

        [Fact]
        public void CanModify_Admin_ShouldReturnTrue()
        {
            ClaimsPrincipal user = CreateClaimsPrincipal(role: Role.Admin.ToString());

            QuizDTO quiz = new();

            bool actual = user.CanModify(quiz);

            Assert.True(actual);
        }

        [Fact]
        public void CanModify_Creator_ShouldReturnTrue()
        {
            int creatorId = 0;

            ClaimsPrincipal user = CreateClaimsPrincipal(id: creatorId.ToString());

            QuizDTO quiz = new() { CreatorId = creatorId };

            bool actual = user.CanModify(quiz);

            Assert.True(actual);
        }

        [Fact]
        public void CanModify_NotCreator_ShouldReturnFalse()
        {
            int userId = 1;
            int creatorId = 0;

            ClaimsPrincipal user = CreateClaimsPrincipal(id: userId.ToString());

            QuizDTO quiz = new() { CreatorId = creatorId };

            bool actual = user.CanModify(quiz);

            Assert.False(actual);
        }
    }
}