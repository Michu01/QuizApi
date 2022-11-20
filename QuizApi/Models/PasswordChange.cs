using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizApi.Models
{
    public class PasswordChange
    {
        [Required]
        [StringLength(32)]
        [NotNull]
        public string? CurrentPassword { get; set; }

        [Required]
        [StringLength(32)]
        [NotNull]
        public string? NewPassword { get; set; }
    }
}
