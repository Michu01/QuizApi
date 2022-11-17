using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;

using QuizApi.Enums;
using QuizApi.JsonConverters;
using QuizApi.Models;

namespace QuizApi.DTOs
{
    [Index(nameof(Name), IsUnique = true)]
    public class UserDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(32)]
        [NotNull]
        public string? Name { get; set; }

        [Required]
        [StringLength(84, MinimumLength = 84)]
        [NotNull]
        [JsonIgnore]
        public string? Password { get; set; }

        public UserRole Role { get; set; }

        [JsonConverter(typeof(JsonDateOnlyConverter))]
        public DateTime JoinDate { get; set; }

        [JsonIgnore]
        public virtual ICollection<FriendshipDTO> Friendships { get; set; } = new List<FriendshipDTO>();

        [JsonIgnore]
        public virtual ICollection<QuestionSetDTO> QuestionSets { get; set; } = new List<QuestionSetDTO>();

        [JsonIgnore]
        public virtual ICollection<FriendshipRequestDTO> FriendshipRequests { get; set; } = new List<FriendshipRequestDTO>();

        [JsonIgnore]
        [NotMapped]
        public IEnumerable<User> Friends => Friendships
            .Select(f => new User(f.MeId == Id ? f.TheyId : f.MeId));

        [JsonIgnore]
        [NotMapped]
        public IEnumerable<User> ReceivedFriendshipRequests => FriendshipRequests
            .Where(f => f.ReceiverId == Id)
            .Select(f => new User(f.SenderId));

        [JsonIgnore]
        [NotMapped]
        public IEnumerable<User> SentFriendshipRequests => FriendshipRequests
            .Where(f => f.SenderId == Id)
            .Select(f => new User(f.ReceiverId));

        public bool IsFriend(int userId) => Friends.Any(user => user.Id == userId);
    }
}
