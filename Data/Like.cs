using System.ComponentModel.DataAnnotations;

namespace Data
{

    public class Like
    {
        [Key]
        public Guid LikeId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public User? User { get; set; }
        [Required]
        public required Guid PostId { get; set; }
        [Required]
        public Post? Post { get; set; }
    }
}