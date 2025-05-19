using System.ComponentModel.DataAnnotations;

namespace Data {
    public class User {
        [Key]
        public Guid UserId { get; set; }
        [Required]
        public required string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string PasswordHash { get; set; }
        public bool IsAdmin { get; set; }
        public byte[]? ProfileImage { get; set; }
        public string Location { get; set; } = "";
        public string Bio { get; set; } = "";
        public ICollection<Like> Likes { get; set; } = [];
        public ICollection<Share> Shares { get; set; } = [];
        public ICollection<Follow> Follows { get; set; } = [];
        public ICollection<Follow> Followers { get; set; } = [];
    }
} 