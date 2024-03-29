﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;

using QuizApi.Enums;
using QuizApi.JsonConverters;

namespace QuizApi.DTOs
{
    [Index(nameof(Name), IsUnique = true)]
    public class QuizDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        [NotNull]
        public string? Name { get; set; }

        [Required]
        [StringLength(256)]
        [NotNull]
        public string? Description { get; set; }

        public Access Access { get; set; }

        [JsonConverter(typeof(JsonDateOnlyConverter))]
        public DateTime CreationDate { get; set; }

        public int CreatorId { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual UserDTO? Creator { get; set; }

        public int CategoryId { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual CategoryDTO? Category { get; set; }

        [NotNull]
        [JsonIgnore]
        public virtual ICollection<QuestionDTO>? Questions { get; set; }
    }
}
