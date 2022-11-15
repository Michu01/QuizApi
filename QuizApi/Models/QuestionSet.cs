using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using QuizApi.Enums;

namespace QuizApi.Models
{
    public class QuestionSet
    {
        [Required]
        [StringLength(64)]
        [NotNull]
        public string? Name { get; set; }

        [Required]
        public QuestionSetAccess Access { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
