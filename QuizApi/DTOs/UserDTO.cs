using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

using QuizApi.Enums;
using QuizApi.JsonConverters;

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

        public bool IsFriend(int id)
        {
            return Friendships.SingleOrDefault(f => f.TheyId == id) is not null;
        }
    }
}
