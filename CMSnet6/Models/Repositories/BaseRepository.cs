using CMSnet6.Models.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CMSnet6.Models.Repositories
{
    public class BaseRepository<T> where T : class
    {
        protected readonly Context _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(Context context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }






        public virtual int Count { get => _dbSet.Count(); }

        public virtual IQueryable<T> All() => _dbSet.AsQueryable();

        public virtual T GetById(object id) => _dbSet.Find(id);

        //
        public virtual IQueryable<T> Get(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (!String.IsNullOrEmpty(includeProperties))
            {
                string[] included = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string prop in included)
                    query = query.Include(prop);
            }

            return orderBy == null ? query : orderBy(query);
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate);

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50)
        {
            int skipCount = index * size;
            IQueryable<T> resetSet = filter != null ? _dbSet.Where(filter) : _dbSet.AsQueryable();
            resetSet = skipCount == 0 ? resetSet.Take(size) : resetSet.Skip(skipCount).Take(size);
            total = resetSet.Count();
            return resetSet.AsQueryable();
        }

        public bool Contains(Expression<Func<T, bool>> predicate) => _dbSet.Any(predicate);



        public virtual void Create(T entity) => _dbSet.Add(entity);



        /*public virtual void Update(T entity)
        {
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry = _context.Entry(entity);
            _dbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }*/



        public virtual void Delete(T entity) => _dbSet.Remove(entity);

        public virtual void DeleteById(object id) => _dbSet.Remove(_dbSet.Find(id));

        public virtual void Delete(Expression<Func<T, bool>> predicate) => _dbSet.RemoveRange(Filter(predicate));



        public virtual IEnumerable<T> ExecuteRawSQL(string query)
            => _dbSet.FromSqlRaw(query).ToList();
            //=> _dbSet.FromSqlInterpolated(query).ToList();
    }
}
