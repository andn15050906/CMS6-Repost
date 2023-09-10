using CMSnet6.Models.EntityModels.UserModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSnet6.Models.EntityModels.PostModels
{
    public class Post
    {
        public int Id { get; set; }

        public User? Author { get; set; }

        [Column(TypeName = "NVARCHAR(3000)")]
        public string? Content { get; set; }

        [Column(TypeName = "NVARCHAR(100)")]
        public string? Title { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastUpdate { get; set; }

        [Column(TypeName = "VARCHAR(45)")]
        public string? Status { get; set; }

        public bool CommentEnabled { get; set; }

        public int CommentCount { get; set; }

        public int ReadCount { get; set; }

        [Column(TypeName = "VARCHAR(45)")]
        public string? Restriction { get; set; }






        public ICollection<PostTag> PostTags { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
