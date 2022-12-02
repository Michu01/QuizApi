using Microsoft.AspNetCore.Identity;

using QuizApi.DTOs;
using QuizApi.Enums;
using QuizApi.Models;
using QuizApi.Repositories;

namespace QuizApi.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUsersRepository usersRepository;

        private readonly IPasswordHasher<UserDTO> passwordHasher;

        private readonly ITokenGenerator tokenGenerator;

        public AuthenticationService(
            IUsersRepository usersRepository, 
            ITokenGenerator tokenGenerator, 
            IPasswordHasher<UserDTO> passwordHasher)
        {
            this.usersRepository = usersRepository;
            this.tokenGenerator = tokenGenerator;
            this.passwordHasher = passwordHasher;
        }

        private void VerifyPassword(UserDTO user, string password)
        {
            if (passwordHasher.VerifyHashedPassword(user, user.Password, password) != PasswordVerificationResult.Success)
            {
                throw new Exception("Invalid password");
            }
        }

        public async Task<Token> SignIn(AuthData authData)
        {
            if (await usersRepository.FindByName(authData.Name) is not UserDTO user)
            {
                throw new Exception($"User with name \"{authData.Name}\" not found");
            }

            VerifyPassword(user, authData.Password);

            return tokenGenerator.Generate(user);
        }

        public async Task<Token> SignUp(AuthData authData)
        {
            UserDTO user = new()
            {
                Name = authData.Name,
                Role = Role.User
            };

            user.Password = passwordHasher.HashPassword(user, authData.Password);

            usersRepository.Add(user);
            await usersRepository.SaveChangesAsync();

            return tokenGenerator.Generate(user);
        }

        public async Task<Token> ChangePassword(int userId, PasswordChange passwordChange)
        {
            if (await usersRepository.Find(userId) is not UserDTO user)
            {
                throw new Exception($"User with id: ${userId} not found");
            }

            VerifyPassword(user, passwordChange.CurrentPassword);

            user.Password = passwordHasher.HashPassword(user, passwordChange.NewPassword);

            await usersRepository.SaveChangesAsync();

            return tokenGenerator.Generate(user);
        }

        public async Task<Token> ChangeUsername(int userId, UsernameChange usernameChange)
        {
            if (await usersRepository.Find(userId) is not UserDTO user)
            {
                throw new Exception($"User with id: ${userId} not found");
            }

            VerifyPassword(user, usernameChange.Password);

            user.Name = usernameChange.Name;

            await usersRepository.SaveChangesAsync();

            return tokenGenerator.Generate(user);
        }
    }
}