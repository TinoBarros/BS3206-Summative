using System.ComponentModel.DataAnnotations;

namespace Data
{
    
    public class Share
    {
        [Key]
        public Guid ShareId { get; set; } 
        [Required]
        public Guid UserId { get; set; }  
        [Required]
        public User? User { get; set; }  
        [Required]
        public Guid PostId { get; set; } 
        [Required]
        public Post? Post { get; set; }   
        public DateTime SharedAt { get; set; } = DateTime.UtcNow; 
    }
}