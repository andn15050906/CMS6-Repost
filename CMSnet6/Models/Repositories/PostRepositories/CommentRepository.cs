using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using CMSnet6.Controllers.DTOs.Comment;
using CMSnet6.Models.DTOs.Post;
using CMSnet6.Models.EntityModels;
using CMSnet6.Models.EntityModels.PostModels;
using CMSnet6.Models.EntityModels.UserModels;
using CMSnet6.Models.DTOs.User;
using CMSnet6.Helpers.FileHandler;

namespace CMSnet6.Models.Repositories.PostRepositories
{
    public class CommentRepository : BaseRepository<Comment>
    {
        public CommentRepository(Context context) : base(context) { }



        public List<CommentDTO>? Get(CommentQueryDTO query)
        {
            List<CommentDTO> result;

            if (query.AuthorId != null)
            {
                result = _context.Comments.Include(c => c.Author)
                    .Where(c => c.Author!.Id == query.AuthorId)
                    .Select(MapExpression).ToList();

                return result;
            }

            if (query.PostId != null)
            {
                result = _context.Comments.Include(c => c.Author)
                    .Where(c => c.Post.Id == query.PostId)
                    .Select(MapExpression).ToList();

                return result;
            }

            if (query.ParentId != null)
            {
                result = _context.Comments.Include(c => c.Author)
                    .Where(c => c.Path.Contains("-" + query.ParentId + "-"))
                    .Select(MapExpression).ToList();

                return result;
            }

            if (query.Date != null)
            {
                result = _context.Comments.Include(c => c.Author)
                    .Where(c => c.CreatedTime > query.Date)
                    .Select(MapExpression).ToList();

                return result;
            }

            if (query.Status != null)
            {
                result = _context.Comments.Include(c => c.Author)
                    .Where(c => c.Status == query.Status)
                    .Select(MapExpression).ToList();

                return result;
            }

            return _context.Comments.Include(p => p.Author)
                    .Select(MapExpression).ToList();
        }

        public CommentDTO? GetById(int id)
        {
            CommentDTO[] result = _context.Comments.Include(c => c.Author)
                .Where(p => p.Id == id)
                .Take(1)
                .Select(MapExpression).ToArray();
            if (result.Length == 0)
                return null;
            return result[0];
        }






        public (CommentDTO?, StatusMessage) Create(CommentPostDTO _, string? email)
        {
            User? user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return (null, StatusMessage.Unauthorized);

            Post? post = _context.Posts.FirstOrDefault(p => p.Id == _.PostId);
            if (post == null)
                return (null, StatusMessage.BadRequest);
            if (post.CommentEnabled == false)
                return (null, StatusMessage.Forbidden);

            string path = "";
            if (_.Parent != null)
            {
                Comment? parent = _context.Comments.Include(c => c.Post).FirstOrDefault(p => p.Id == _.Parent);

                if (parent.Post.Id != _.PostId)
                    return (null, StatusMessage.BadRequest);
                if (parent != null)
                    path = parent.Path + '-' + _.Parent + '-';
            }

            //path doesn't contain post's id & comment's id
            Comment comment = new()
            {
                Author = user,
                Post = post,
                Path = path,
                Content = _.Content,
                CreatedTime = DateTime.Now,
                LastUpdate = DateTime.Now
            };

            //add files to the end
            comment.Content += SaveFilesAndGetInfo(_.Files, new FileHandler());

            _context.Comments.Add(comment);

            //update the post
            post.CommentCount++;

            _context.SaveChanges();

            return (Map(comment), StatusMessage.Created);
        }

        public (CommentDTO?, StatusMessage) Update(CommentPutDTO _, string? email)
        {
            User? user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return (null, StatusMessage.Unauthorized);

            Comment? comment = _context.Comments.Include(c => c.Author).Include(c => c.Post)
                .FirstOrDefault(c => c.Id == _.Id);
            if (comment == null)
                return (null, StatusMessage.BadRequest);
            if (comment.Author.Id != user.Id)
                return (null, StatusMessage.Unauthorized);

            comment.LastUpdate = DateTime.Now;

            FileHandler handler = new();
            DeleteFiles(comment.Content, handler);
            comment.Content = _.Content + SaveFilesAndGetInfo(_.Files, handler);

            _context.SaveChanges();

            return (Map(comment), StatusMessage.Ok);
        }

        public StatusMessage Delete(int id, string? email)
        {
            Comment? comment = _context.Comments.Include(p => p.Post).Include(p => p.Author)
                .FirstOrDefault(p => p.Id == id);
            if (comment == null)
                return StatusMessage.BadRequest;
            if (comment.Author.Email != email)
                return StatusMessage.Unauthorized;

            DeleteFiles(comment.Content, new FileHandler());

            //update the post
            comment.Post!.CommentCount--;

            _context.Comments.Remove(comment);

            return StatusMessage.Ok;
        }






        private static readonly Expression<Func<Comment, CommentDTO>> MapExpression = _ => new CommentDTO
        {
            Id = _.Id,
            PostId = _.Post.Id,
            Path = _.Path,
            Content = _.Content,
            CreatedTime = _.CreatedTime,
            LastUpdate = _.LastUpdate,
            Status = _.Status,

            Author = new UserMinDTO
            {
                Id = _.Author.Id,
                UserName = _.Author.UserName,
                Avatar = _.Author.Avatar
            }
        };

        private static CommentDTO Map(Comment _)
        {
            return new CommentDTO
            {
                Id = _.Id,
                PostId = _.Post.Id,
                Path = _.Path,
                Content = _.Content,
                CreatedTime = _.CreatedTime,
                LastUpdate = _.LastUpdate,
                Status = _.Status,

                Author = new UserMinDTO
                {
                    Id = _.Author.Id,
                    UserName = _.Author.UserName,
                    Avatar = _.Author.Avatar
                }
            };
        }

        private static string SaveFilesAndGetInfo(List<IFormFile>? files, FileHandler handler)
        {
            if (files != null)
                if (files.Count > 0)
                    return '*' + handler.SaveFiles(files, UploadDir.Comment);
            return "";
        }

        private static void DeleteFiles(string content, FileHandler handler)
        {
            int index = content.LastIndexOf('*');
            if (index != -1)
                handler.DeleteFiles(content[(index + 1)..], UploadDir.Comment);
        }
    }
}
