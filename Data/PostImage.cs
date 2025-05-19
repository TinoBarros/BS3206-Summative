using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    public class PostImage
    {
        [Key]
        public Guid PostImageId { get; set; }
        public Guid PostId { get; set; }
        public Post? Post { get; set; }
        public required byte[] ImageData { get; set; }
        public required string ContentType { get; set; }
    }
}