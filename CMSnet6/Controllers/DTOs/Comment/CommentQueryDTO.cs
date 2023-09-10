namespace CMSnet6.Controllers.DTOs.Comment
{
    public class CommentQueryDTO
    {
        public string? AuthorId { get; set; }
        public int? PostId { get; set; }
        public int? ParentId { get; set; }
        public DateTime? Date { get; set; }
        public string? Status { get; set; }
    }
}
