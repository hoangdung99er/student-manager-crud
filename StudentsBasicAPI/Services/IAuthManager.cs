using StudentsBasicAPI.DTO;

namespace StudentsBasicAPI.Services
{
    public interface IAuthManager
    {
        Task<bool> ValidateUser(LoginDTO userDTO);

        Task<string> CreateToken();

        string GetTokenInfo(string token);
    }
}
