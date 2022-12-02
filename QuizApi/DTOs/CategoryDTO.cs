using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;

namespace QuizApi.DTOs
{
    [Index(nameof(Name), IsUnique = true)]
    public class CategoryDTO
    {
        [Key]
        public int Id { get; set; }

        [Required] 
        [StringLength(64)]
        [NotNull]
        public string? Name { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual IEnumerable<QuizDTO>? Quizes { get; set; }
    }
}
