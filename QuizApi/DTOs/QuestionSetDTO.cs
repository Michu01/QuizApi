using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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
        public virtual UserDTO? Creator { get; set; }

        public int CategoryId { get; set; }

        [NotNull]
        public virtual QuestionSetCategoryDTO? Category { get; set; }

        [NotNull]
        public virtual ICollection<QuestionDTO>? Questions { get; set; }
    }
}
