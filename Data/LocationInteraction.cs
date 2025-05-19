using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class LocationInteraction
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid LocationId { get; set; }
        public bool IsLiked { get; set; }
        public bool IsDisliked { get; set; }
    }
} 