using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using QuizApi.Enums;

namespace QuizApi.DTOs
{
    public class QuestionSetDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        [NotNull]
        public string? Name { get; set; }

        public QuestionSetAccess Access { get; set; }

        public DateTime CreationDate { get; set; }

        public int CreatorId { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual UserDTO? Creator { get; set; }

        public int CategoryId { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual QuestionSetCategoryDTO? Category { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual ICollection<QuestionDTO>? Questions { get; set; }
    }
}
