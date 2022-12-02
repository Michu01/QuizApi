using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace QuizApi.DTOs
{
    public class FriendshipDTO
    {
        public int FirstUserId { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual UserDTO? FirstUser { get; set; }

        public int SecondUserId { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual UserDTO? SecondUser { get; set; }
    }
}
