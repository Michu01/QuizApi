using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Enums;
using QuizApi.Extensions;
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

        private string GenerateToken(UserDTO user)
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

            JwtSecurityToken securityToken = new(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                DateTime.Now,
                DateTime.Now.AddMinutes(15),
                credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return token;
        }

        public async Task<(UserDTO, string)> SignIn(UserAuthData authData)
        {
            if (await dbContext.FindUserByName(authData.Name) is not UserDTO user)
            {
                throw new Exception("User not found");
            }

            if (passwordHasher.VerifyHashedPassword(user, user.Password!, authData.Password!) != PasswordVerificationResult.Success)
            {
                throw new Exception("Invalid password");
            }

            string token = GenerateToken(user);

            return (user, token);
        }

        public async Task<(UserDTO, string)> SignUp(UserAuthData authData)
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

            string token = GenerateToken(user);

            return (user, token);
        }
    }
}
