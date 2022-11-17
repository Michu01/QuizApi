using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;

using QuizApi.Enums;

namespace QuizApi.DTOs
{
    public class QuestionDTO
    {
        [Key]
        public int Id { get; set; }

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

        public QuestionAnswer CorrectAnswer { get; set; }

        public int QuestionSetId { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual QuestionSetDTO? QuestionSet { get; set; }
    }
}