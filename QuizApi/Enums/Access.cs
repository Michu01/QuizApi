using System.Text.Json.Serialization;

namespace QuizApi.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Access
    {
        Private, Friends, Public
    }
}
