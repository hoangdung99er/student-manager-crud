using System.ComponentModel.DataAnnotations;

namespace StudentsBasicAPI.DTO
{
    public class LoginDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Your password is limited to {2} to {1} characters", MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class ProfileUserDTO
    {
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "First Name Is Too Long")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "Last Name Is Too Long")]
        public string LastName { get; set; }

    }

    public class UserDTO: LoginDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public ICollection<string> Roles { get; set; }
    }
}
