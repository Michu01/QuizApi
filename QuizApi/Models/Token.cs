using System.ComponentModel.DataAnnotations;

namespace QuizApi.Models
{
    public record Token
    {
        public Token(string value, DateTime expirationTime)
        {
            Value = value;
            ExpirationTime = expirationTime;
        }

        [Required]
        public string Value { get; }

        [Required]
        public DateTime ExpirationTime { get; }
    }
}
