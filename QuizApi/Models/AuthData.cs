using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizApi.Models
{
    public class AuthData
    {
        [Required]
        [StringLength(32)]
        [NotNull]
        public string? Name { get; set; }

        [Required]
        [StringLength(32)]
        [NotNull]
        public string? Password { get; set; }
    }
}
