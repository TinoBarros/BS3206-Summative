using System.ComponentModel.DataAnnotations;

namespace Data {
    public class User {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string PasswordHash { get; set; }
        public byte[]? ProfileImage { get; set; }
        public string? Location { get; set; }
        public string? Bio { get; set; }
        public bool IsAdmin { get; set; }
    }
} 