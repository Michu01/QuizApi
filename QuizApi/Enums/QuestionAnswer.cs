using System.Text.Json.Serialization;

namespace QuizApi.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum QuestionAnswer
    {
        A, B, C, D
    }
}
