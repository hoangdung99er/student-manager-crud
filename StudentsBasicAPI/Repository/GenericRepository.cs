using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using StudentsBasicAPI.IRepository;
using StudentsBasicAPI.Models;

namespace StudentsBasicAPI.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDBContext _context;
        private readonly DbSet<T> _db;

        // Dependency Injection will require to create a constructor
        // context was the one being injected in.
        public GenericRepository(ApplicationDBContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task Delete(int id)
        {
            var entity = await _db.FindAsync(id);
            _db.Remove(entity);
        }

        public void DeleteRage(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }

        // Expression -> allow to put in a Lambda expression
        // Because then Lambda Expression would allow us to see lambda arrow 
        public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null)
        {
            IQueryable<T> query = _db;
            if(includes != null)
            {
                foreach(var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            // any record is retrieved is not being tracked
            // dismiss tracking LINQ
            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
        {
            IQueryable<T> query = _db;
            // query where conditions is provied
            if(expression != null) 
            {
                query = query.Where(expression);
            }

            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            if(orderBy != null)
            {
                query = orderBy(query);
            }

            // any record is retrieved is not being tracked
            // dismiss tracking LINQ
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task Insert(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public async void Update(T entity)
        {
            // Two part operations,
            // One, going to attach the entity to the DB
            
            // Check if it already have and check any difference between it and what you have in the database
            _db.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
