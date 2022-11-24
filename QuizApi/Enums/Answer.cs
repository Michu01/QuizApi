using System.Text.Json.Serialization;

namespace QuizApi.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Answer
    {
        A, B, C, D
    }
}
