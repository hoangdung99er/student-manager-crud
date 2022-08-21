namespace StudentsBasicAPI.Models
{
    public class ClassStudent
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public virtual IList<Student> Students { get; set; }
    }
}
