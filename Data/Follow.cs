using System.ComponentModel.DataAnnotations;

namespace Data
{
    
    public class Follow
    {
        [Key]
        public Guid FollowId { get; set; } 
        [Required]
        public Guid UserId { get; set; }  
        [Required]
        public required User User { get; set; }  
        [Required]
        public Guid FollowedUserId { get; set; }  
        [Required]
        public required User FollowedUser { get; set; }  
    }
}