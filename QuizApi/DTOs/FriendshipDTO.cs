using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace QuizApi.DTOs
{
    public class FriendshipDTO
    {
        public int MeId { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual UserDTO? Me { get; set; }

        public int TheyId { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual UserDTO? They { get; set; }
    }
}
