namespace CMSnet6.Controllers.DTOs.Comment
{
    public class CommentPutDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
