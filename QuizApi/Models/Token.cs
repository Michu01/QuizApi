namespace QuizApi.Models
{
    public record Token
    {
        public Token(string value, DateTime expirationTime)
        {
            Value = value;
            ExpirationTime = expirationTime;
        }

        public string Value { get; }

        public DateTime ExpirationTime { get; }
    }
}
