using System;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class PostImage
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public byte[] ImageData { get; set; } = [];
        public string ContentType { get; set; } = "image/jpeg";

        public Post Post { get; set; } = null!;
    }
}