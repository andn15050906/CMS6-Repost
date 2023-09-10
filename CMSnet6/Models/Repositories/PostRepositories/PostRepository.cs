using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;

using CMSnet6.Models.EntityModels;
using CMSnet6.Models.EntityModels.PostModels;
using CMSnet6.Controllers.DTOs.Post;
using CMSnet6.Models.DTOs.Post;
using CMSnet6.Models.DTOs.User;
using CMSnet6.Models.EntityModels.UserModels;
using CMSnet6.Helpers.FileHandler;

namespace CMSnet6.Models.Repositories.PostRepositories
{
    public class PostRepository : BaseRepository<Post>
    {
        public PostRepository(Context context) : base(context) { }



        public List<PostDTO>? Get(PostQueryDTO query)
        {
            List<PostDTO> result;

            if (query.AuthorId != null)
            {
                result = _context.Posts.Include(p => p.Author).Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                    .Where(p => p.Author!.Id == query.AuthorId)
                    .Select(MapExpression).ToList();

                return result;
            }


            if (query.Tags != null)
            {
                //~ Select many-many Where many
                List<string> tagNames = DecodeArray(query.Tags);
                if (tagNames == null)
                    return null;

                result = new List<PostDTO>();
                string sqlTagName = "";
                for (int i = 0; i < tagNames.Count; i++)
                {
                    sqlTagName += $"'{tagNames[i]}'";
                    if (i < tagNames.Count - 1)
                        sqlTagName += ",";
                }

                string queryString =
                    "SELECT * FROM Posts WHERE Id IN (SELECT PostID FROM PostTag " +
                    $"WHERE TagId IN (SELECT Id FROM Tags WHERE Name IN ({sqlTagName})) " +
                    $"GROUP BY PostId HAVING COUNT(TagId) = {tagNames.Count})";

                IEnumerable<Post> res = ExecuteRawSQL($"{queryString}");
                foreach (Post post in res) {
                    _context.Entry(post).Reference(p => p.Author).Load();
                    _context.Entry(post).Collection(p => p.PostTags).Query().Include(pt => pt.Tag).Load();
                    result.Add(Map(post));
                };
                return result;
            }

            if (query.Date != null)
            {
                result = _context.Posts.Include(p => p.Author).Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                    .Where(p => p.CreatedTime > query.Date)
                    .Select(MapExpression).ToList();

                return result;
            }

            return _context.Posts.Include(p => p.Author).Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                    .Select(MapExpression).ToList();
        }

        public PostDTO? GetById(int id)
        {
            PostDTO[] result = _context.Posts.Include(p => p.Author).Include(p => p.PostTags).ThenInclude(pt => pt.Tag)
                .Where(p => p.Id == id)
                .Take(1)
                .Select(MapExpression).ToArray();
            if (result.Length == 0)
                return null;
            return result[0];
        }






        public (PostDTO?, StatusMessage) Create(PostPostDTO _, string? email)
        {
            if (_context.Posts.Any(p => p.Title == _.Title))
                return (null, StatusMessage.Conflict);
            List<string> tagNames = DecodeArray(_.Tags);
            if (tagNames == null)
                return (null, StatusMessage.BadRequest);
            User? user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return (null, StatusMessage.Unauthorized);

            Post post = new()
            {
                Author = user,
                Content = _.Content,
                Title = _.Title,
                CreatedTime = DateTime.Now,
                LastUpdate = DateTime.Now,
                CommentEnabled = _.CommentEnabled,
                Restriction = _.Restriction,
                PostTags = new List<PostTag>()
            };
            _context.Posts.Add(post);

            //add files to the end
            post.Content += SaveFilesAndGetInfo(_.Files, new FileHandler());

            foreach (string tagName in tagNames)
            {
                bool addingTag  = false;
                Tag? tag = _context.Tags.FirstOrDefault(t => t.Name == tagName);
                if (tag == null)
                {
                    addingTag = true;
                    tag = new Tag { Name = tagName };
                }

                PostTag newTP = new() { Post = post, Tag = tag };
                tag.PostTags = new List<PostTag> { newTP };
                post.PostTags.Add(newTP);

                if (addingTag)
                    _context.Tags.Add(tag);
            }
            _context.SaveChanges();

            return (Map(post), StatusMessage.Created);
        }

        public (PostDTO?, StatusMessage) Update(PostPutDTO _, string? email)
        {
            if (_context.Posts.Any(p => p.Title == _.Title && p.Id != _.Id))
                return (null, StatusMessage.Conflict);
            List<string> tagNames = DecodeArray(_.Tags);
            if (tagNames == null)
                return (null, StatusMessage.BadRequest);
            User? user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return (null, StatusMessage.Unauthorized);

            Post? post = _context.Posts.Include(p => p.Author).Include(p => p.PostTags)
                .FirstOrDefault(p => p.Id == _.Id);
            if (post == null)
                return (null, StatusMessage.BadRequest);
            if (post.Author.Id != user.Id)
                return (null, StatusMessage.Unauthorized);

            post.Title = _.Title;
            post.LastUpdate = DateTime.Now;
            post.Status = _.Status;
            post.CommentEnabled = _.CommentEnabled;

            FileHandler handler = new();
            DeleteFiles(post.Content, handler);
            post.Content = _.Content + SaveFilesAndGetInfo(_.Files, handler);

            //delete all old PTs, add all as new
            post.PostTags.Clear();
            post.PostTags = new List<PostTag>();
            foreach (string tagName in tagNames)
            {
                //add tag if new
                Tag? tag = _context.Tags.FirstOrDefault(ele => ele.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag { Name = tagName };
                    _context.Tags.Add(tag);
                }

                //relation will be added
                PostTag newNT = new () { Post = post, Tag = tag };
                tag.PostTags = new List<PostTag> { newNT };
                post.PostTags.Add(newNT);
            }
            _context.SaveChanges();

            return (Map(post), StatusMessage.Ok);
        }

        public StatusMessage Delete(int id, string? email)
        {
            Post? post = _context.Posts.Include(p => p.Author).FirstOrDefault(p => p.Id == id);
            if (post == null)
                return StatusMessage.BadRequest;
            if (post.Author.Email != email)
                return StatusMessage.Unauthorized;

            DeleteFiles(post.Content, new FileHandler());
            _context.Posts.Remove(post);
            return StatusMessage.Ok;
        }






        public void IncreaseReadCount(int postId)
        {
            string queryString = $"UPDATE Posts SET ReadCount = ReadCount + 1 WHERE Id = {postId}";
            _context.Database.ExecuteSqlRaw(queryString);
        }





        private static readonly Expression<Func<Post, PostDTO>> MapExpression = _ => new PostDTO
        {
            Id = _.Id,
            Content = _.Content,
            Title = _.Title,
            CreatedTime = _.CreatedTime,
            LastUpdate = _.LastUpdate,
            Status = _.Status,
            CommentEnabled = _.CommentEnabled,
            CommentCount = _.CommentCount,
            ReadCount = _.ReadCount,
            Restriction = _.Restriction,

            Author = new UserMinDTO
            {
                Id = _.Author.Id,
                UserName = _.Author.UserName,
                Avatar = _.Author.Avatar
            },
            Tags = _.PostTags.Select(pt => pt.Tag.Name)
        };

        private static PostDTO Map(Post _)
        {
            return new PostDTO
            {
                Id = _.Id,
                Content = _.Content,
                Title = _.Title,
                CreatedTime = _.CreatedTime,
                LastUpdate = _.LastUpdate,
                Status = _.Status,
                CommentEnabled = _.CommentEnabled,
                CommentCount = _.CommentCount,
                ReadCount = _.ReadCount,
                Restriction = _.Restriction,

                Author = new UserMinDTO
                {
                    Id = _.Author.Id,
                    UserName = _.Author.UserName,
                    Avatar = _.Author.Avatar
                },
                Tags = _.PostTags.Select(pt => pt.Tag.Name)
            };
        }

        private static List<string> DecodeArray(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<List<string>>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string SaveFilesAndGetInfo(List<IFormFile>? files, FileHandler handler)
        {
            if (files != null)
                if (files.Count > 0)
                    return '*' + handler.SaveFiles(files, UploadDir.Post);
            return "";
        }

        private static void DeleteFiles(string content, FileHandler handler)
        {
            int index = content.LastIndexOf('*');
            if (index != -1)
                handler.DeleteFiles(content[(index + 1)..], UploadDir.Post);
        }
    }
}
