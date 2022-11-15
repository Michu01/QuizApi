using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace QuizApi.DTOs
{
    public class FriendshipDTO
    {
        [ForeignKey(nameof(MeId))]
        public int MeId { get; set; }

        [Required]
        [NotNull]
        public UserDTO? Me { get; set; }

        [ForeignKey(nameof(TheyId))]
        public int TheyId { get; set; }

        [Required]
        [NotNull]
        public UserDTO? They { get; set; }
    }
}
