using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class LocationInteraction
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int LocationId { get; set; }
        public bool IsLiked { get; set; }
        public bool IsDisliked { get; set; }
    }
} 