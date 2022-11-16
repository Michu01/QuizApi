using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace QuizApi.DTOs
{
    public class FriendshipRequestDTO
    {
        [ForeignKey(nameof(SenderId))]
        public int SenderId { get; set; }

        [Required]
        [NotNull]
        [JsonIgnore]
        public UserDTO? Sender { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public int ReceiverId { get; set; }

        [Required]
        [NotNull]
        [JsonIgnore]
        public UserDTO? Receiver { get; set; }
    }
}
