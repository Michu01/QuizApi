using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizApi.Models
{
    public class Category
    {
        [Required]
        [StringLength(64)]
        [NotNull]
        public string? Name { get; set; }
    }
}
