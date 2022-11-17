using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Enums;
using QuizApi.Models;

namespace QuizApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly QuizDbContext dbContext;

        private readonly IPasswordHasher<UserDTO> passwordHasher;

        private readonly IConfiguration configuration;

        public AuthService(QuizDbContext dbContext, IConfiguration configuration, IPasswordHasher<UserDTO> passwordHasher)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
            this.passwordHasher = passwordHasher;
        }

        private Token GenerateToken(UserDTO user)
        {
            string key = configuration["Jwt:Key"]!;

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(key));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            IEnumerable<Claim> claims = new Claim[]
            {
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Role, user.Role.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            DateTime expirationDate = DateTime.Now.AddMinutes(15);

            JwtSecurityToken securityToken = new(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                DateTime.Now,
                expirationDate,
                credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new Token(token, expirationDate);
        }

        public async Task<Token> SignIn(UserAuthData authData)
        {
            if (await dbContext.FindUserByName(authData.Name) is not UserDTO user)
            {
                throw new Exception("User not found");
            }

            if (passwordHasher.VerifyHashedPassword(user, user.Password!, authData.Password!) != PasswordVerificationResult.Success)
            {
                throw new Exception("Invalid password");
            }

            Token token = GenerateToken(user);

            return token;
        }

        public async Task<Token> SignUp(UserAuthData authData)
        {
            if (await dbContext.FindUserByName(authData.Name) is not null)
            {
                throw new Exception($"User with name: \"{authData.Name}\" already exists");
            }

            UserDTO user = new()
            {
                Name = authData.Name,
                Role = UserRole.User
            };

            user.Password = passwordHasher.HashPassword(user, authData.Password!);

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            Token token = GenerateToken(user);

            return token;
        }
    }
}
