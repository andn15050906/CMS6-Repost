using CMSnet6.Models.DTOs.User;

namespace CMSnet6.Models.DTOs.Post
{
    public class PostDTO
    {
        public int Id { get; set; }

        public UserMinDTO Author { get; set; } = null!;

        public string? Content { get; set; }

        public string Title { get; set; } = null!;

        public DateTime CreatedTime { get; set; }

        public DateTime LastUpdate { get; set; }

        public string? Status { get; set; } = null!;

        public bool CommentEnabled { get; set; }

        public int CommentCount { get; set; }

        public int ReadCount { get; set; }

        public string? Restriction { get; set; } = null!;






        public IEnumerable<string> Tags { get; set; } = null!;
        //public ICollection<string> Comments { get; set; } = null!;
    }
}
