using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;

using QuizApi.Enums;

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

        public DateTime JoinDate { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual ICollection<FriendshipDTO>? Friendships { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual ICollection<QuestionSetDTO>? QuestionSets { get; set; }

        public bool IsFriend(int id)
        {
            return Friendships.SingleOrDefault(f => f.TheyId == id) is not null;
        }
    }
}
