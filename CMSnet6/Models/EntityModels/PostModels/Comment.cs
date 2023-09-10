using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

using CMSnet6.Models.EntityModels.UserModels;

namespace CMSnet6.Models.EntityModels.PostModels
{
    public class Comment
    {
        public int Id { get; set; }

        public User? Author { get; set; }

        public Post? Post { get; set; }

        [Column(TypeName = "VARCHAR(45)")]
        public string? Path { get; set; }

        [Column(TypeName = "NVARCHAR(255)")]
        public string? Content { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastUpdate { get; set; }

        public string? Status { get; set; }
    }
}
