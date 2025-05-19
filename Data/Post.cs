using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class Post
    {
        [Key]
        public Guid PostId { get; set; }
        [Required]
        public required User User { get; set; }
        public required Guid UserId { get; set; }
        [Required]
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public ICollection<Like> Likes { get; set; } = [];
        public ICollection<Share> Shares { get; set; } = [];
        public int Comments { get; set; } = 0;
        public Guid? ParentPostId { get; set; }
        public Post? ParentPost { get; set; }
        public ICollection<Post> CommentsList { get; set; } = [];
        public List<PostImage> Images { get; set; } = [];
    }
}