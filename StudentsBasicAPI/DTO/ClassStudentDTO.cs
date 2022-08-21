using System.ComponentModel.DataAnnotations;

namespace StudentsBasicAPI.DTO
{
    public class CreateClassStudentDTO
    {
        // data annotations
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "Class Name Is Too Long")]
        public string ClassName { get; set; }

    }

    // non-operation specific
    public class ClassStudentDTO : CreateClassStudentDTO
    {
        public int Id { get; set; }

        public IList<StudentDTO> Students { get; set; }
    }

    public class UpdateClassStudentDTO: CreateClassStudentDTO
    {
        public IList<CreateStudentDTO> Students { get; set; }

    }
}
