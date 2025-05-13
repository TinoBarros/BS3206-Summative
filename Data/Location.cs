using System;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? MapUrl { get; set; }
        public int ThumbsUp { get; set; } = 0;
        public int ThumbsDown { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 