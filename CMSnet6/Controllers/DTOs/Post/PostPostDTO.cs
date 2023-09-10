namespace CMSnet6.Controllers.DTOs.Post
{
    public class PostPostDTO
    {
        public string Content { get; set; }
        public string Title { get; set; }
        public List<IFormFile>? Files { get; set; }
        public bool CommentEnabled { get; set; }
        public string? Restriction { get; set; }

        public string Tags { get; set; }
    }
}
