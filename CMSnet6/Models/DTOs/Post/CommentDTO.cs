using CMSnet6.Models.DTOs.User;

namespace CMSnet6.Models.DTOs.Post
{
    public class CommentDTO
    {
        public int Id { get; set; }

        public UserMinDTO Author { get; set; } = null!;

        public int PostId { get; set; }

        public string Path { get; set; }

        public string Content { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime LastUpdate { get; set; }

        public string? Status { get; set; }
    }
}
