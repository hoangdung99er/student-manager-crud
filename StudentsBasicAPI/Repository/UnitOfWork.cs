
using StudentsBasicAPI.IRepository;
using StudentsBasicAPI.Models;

namespace StudentsBasicAPI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _context;

        private IGenericRepository<Student> _students;
        private IGenericRepository<ClassStudent> _classes;

        public UnitOfWork(ApplicationDBContext context)
        {
            _context = context;
        }

        public IGenericRepository<Student> Students => _students ??= new GenericRepository<Student>(_context);

        public IGenericRepository<ClassStudent> Classes => _classes ??= new GenericRepository<ClassStudent>(_context);

        public void Dispose()
        {
            // Like a GC is just seeing when done or when the operations
            // are done -> free of memory

            // When Dispose() called -> dispose context 
            // meaning kill any memory that the connection to the database was using the connection
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
