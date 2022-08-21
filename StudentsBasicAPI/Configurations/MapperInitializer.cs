using AutoMapper;
using StudentsBasicAPI.DTO;
using StudentsBasicAPI.Models;

namespace StudentsBasicAPI.Configurations
{
    public class MapperInitializer: Profile
    {
        public MapperInitializer()
        {
            CreateMap<ClassStudent, ClassStudentDTO>().ReverseMap();
            CreateMap<ClassStudent, CreateClassStudentDTO>().ReverseMap();
            CreateMap<Student, StudentDTO>().ReverseMap();
            CreateMap<Student, CreateStudentDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, ProfileUserDTO>().ReverseMap();
        }
    }
}
