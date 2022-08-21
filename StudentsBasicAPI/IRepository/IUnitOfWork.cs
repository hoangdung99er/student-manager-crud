
using StudentsBasicAPI.Models;

namespace StudentsBasicAPI.IRepository
{
    public interface IUnitOfWork: IDisposable
    {
        // register
        IGenericRepository<Student> Students { get; }
        IGenericRepository<ClassStudent> Classes { get; }
        Task Save();
    }
}
