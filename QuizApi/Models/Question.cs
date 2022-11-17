using QuizApi.Enums;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizApi.Models
{
    public class Question
    {
        [Required]
        [StringLength(256)]
        [NotNull]
        public string? Contents { get; set; }

        [Required]
        [StringLength(64)]
        [NotNull]
        public string? AnswerA { get; set; }

        [Required]
        [StringLength(64)]
        [NotNull]
        public string? AnswerB { get; set; }

        [Required]
        [StringLength(64)]
        [NotNull]
        public string? AnswerC { get; set; }

        [Required]
        [StringLength(64)]
        [NotNull]
        public string? AnswerD { get; set; }

        [Required]
        public QuestionAnswer CorrectAnswer { get; set; }
    }
}
