using Microsoft.EntityFrameworkCore;
using StudentsBasicAPI.Models;

namespace StudentsBasicAPI.Data
{
    public class SeedData
    {
        //private static ApplicationDBContext _dbContext;
        //public SeedData(ApplicationDBContext dbContext)
        //{
        //    _dbContext = dbContext;
        //}
        public static void Initialize(IServiceProvider serviceProvider)
        {
            //if(_dbContext.Students.Any())
            //{
            //    return;
            //}
            //_dbContext.Students.AddRange(
            //    new Student { StudentClass = "Test", StudentName = "Test", Age = 18, Grade = 5.8, MyProperty = Student.StudentRank.Bad },
            //    new Student { StudentClass = "Dummy", StudentName = "Dummy", Age = 22, Grade = 9.8, MyProperty = Student.StudentRank.Excellent }
            //    );
            //_dbContext.SaveChanges();
            using (var context = new ApplicationDBContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDBContext>>()))
            {
                if (context.Students.Any() || context.Classes.Any())
                    return;
               
                context.Classes.AddRange(
                    new ClassStudent
                    {
                        Id = 1,
                        ClassName = "10D5",
                    },
                    new ClassStudent
                    {
                        Id = 2,
                        ClassName = "12D1",
                    }
                );
                context.Students.AddRange(
                    new Student { StudentName = "Test", Age = 18, Grade = 5, ClassId = 1 },
                    new Student { StudentName = "Dummy", Age = 22, Grade = 9, ClassId = 2 }
                );
                context.SaveChanges();
            }
        }
    }
}
