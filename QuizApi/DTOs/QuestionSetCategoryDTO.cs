using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;

namespace QuizApi.DTOs
{
    [Index(nameof(Name), IsUnique = true)]
    public class QuestionSetCategoryDTO
    {
        [Key]
        public int Id { get; set; }

        [Required] 
        [StringLength(64)]
        [NotNull]
        public string? Name { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual IEnumerable<QuestionSetDTO>? QuestionSets { get; set; }
    }
}
