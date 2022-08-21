using System.ComponentModel.DataAnnotations;

namespace StudentsBasicAPI.DTO
{
    public class CreateStudentDTO
    {
        [Required]
        [StringLength(maximumLength: 150, ErrorMessage = "Hotel Name Is Too Long")]
        public string StudentName { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Please enter valid age")]
        public int Age { get; set; }
        [Required]
        //[RegularExpression("([1-10]+)", ErrorMessage = "Please enter valid Grade")]
        [Range(1,10)]
        public double Grade { get; set; }
        //[Required]
        public int ClassId { get; set; }
    }

    public class StudentDTO : CreateStudentDTO
    {
        public int Id { get; set; }

        public ClassStudentDTO ClassStudent { get; set; }
    }

    public class UpdateStudentDTO : CreateStudentDTO
    {
    }
}
