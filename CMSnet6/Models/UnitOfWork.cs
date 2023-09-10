using CMSnet6.Models.EntityModels;
using CMSnet6.Models.Repositories.PostRepositories;
using CMSnet6.Models.Repositories.UserRepositories;

namespace CMSnet6.Models
{
    public class UnitOfWork
    {
        private readonly Context _context;

        private UserRepository? userRepo;
        private PostRepository? postRepo;
        private CommentRepository? commentRepo;



        public UnitOfWork(Context context)
        {
            _context = context;
        }

        public void Save() => _context.SaveChanges();






        //in-UoW-singleton
        public UserRepository UserRepo
        {
            get => userRepo ?? (userRepo = new UserRepository(_context));
        }

        public PostRepository PostRepo
        {
            get => postRepo ?? (postRepo = new PostRepository(_context));
        }

        public CommentRepository CommentRepo
        {
            get => commentRepo ?? (commentRepo = new CommentRepository(_context));
        }
    }
}
