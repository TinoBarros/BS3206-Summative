using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class Notification
    {
        [Key]
        public Guid NotificationId { get; set; }
        [Required]
        public required Guid SenderId { get; set; }
        public User? Sender { get; set; }
        [Required]
        public required Guid ReceiverId { get; set; }
        [Required]
        public User? Receiver { get; set; }
        [Required]
        public required string Text { get; set; }
        public required string Path { get; set; }
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}