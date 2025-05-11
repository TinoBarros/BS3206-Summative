using System;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        public User? User { get; set; }

        public int? UserId { get; set; }

        [Required]
        public string Subject { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public ICollection<Like> Likes { get; set; } = new List<Like>();

        public int Comments { get; set; } = 0;

        public int? ParentPostId { get; set; }

        public Post? ParentPost { get; set; }

        public ICollection<Post> CommentsList { get; set; } = new List<Post>();
        public List<PostImage> Images { get; set; } = new();

    }
}