namespace CMSnet6.Controllers.DTOs.Comment
{
    public class CommentPostDTO
    {
        public int PostId { get; set; }
        public int? Parent { get; set; }
        public string Content { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
