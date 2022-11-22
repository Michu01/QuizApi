using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace QuizApi.DTOs
{
    public class FriendshipRequestDTO
    {
        public int SenderId { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual UserDTO? Sender { get; set; }

        public int ReceiverId { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual UserDTO? Receiver { get; set; }
    }
}
