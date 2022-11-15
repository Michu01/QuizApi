using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizApi.Models
{
    public class QuestionSetCategory
    {
        [Required]
        [StringLength(64)]
        [NotNull]
        public string? Name { get; set; }
    }
}
