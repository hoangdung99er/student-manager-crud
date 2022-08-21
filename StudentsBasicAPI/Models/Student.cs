using System.ComponentModel.DataAnnotations.Schema;

namespace StudentsBasicAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public int Age { get; set; }
        public double Grade { get; set; }

        [ForeignKey(nameof(ClassStudent))]
        public int ClassId { get; set; }

        public ClassStudent ClassStudent { get; set; }
    }
}
