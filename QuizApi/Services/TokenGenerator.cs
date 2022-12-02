using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using QuizApi.DTOs;
using QuizApi.Models;

namespace QuizApi.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration configuration;

        public TokenGenerator(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Token Generate(UserDTO user)
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
    }
}
