using System;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    
    public class Like
    {
        [Key]
        public int LikeId { get; set; } 
        [Required]
        public int UserId { get; set; }  
        [Required]
        public User? User { get; set; }  
        [Required]
        public int PostId { get; set; } 
        [Required]
        public Post? Post { get; set; }   
        public DateTime LikedAt { get; set; } = DateTime.UtcNow; 
    }
}