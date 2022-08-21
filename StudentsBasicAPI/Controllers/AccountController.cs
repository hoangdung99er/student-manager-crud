using AutoMapper;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StudentsBasicAPI.DTO;
using StudentsBasicAPI.IRepository;
using StudentsBasicAPI.Models;
using StudentsBasicAPI.Services;

namespace StudentsBasicAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController: ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthManager _authManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private ProducerConfig _config;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<AccountController> logger, IMapper mapper, IAuthManager authManager, ProducerConfig config)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _authManager = authManager;
            _config = config;
            _unitOfWork = unitOfWork;
        }

        //[HttpGet]
        //public Dictionary<string, string> DecodeToken()
        //{
        //    string authHeader = Request.Headers["Authorization"];
        //    authHeader = authHeader.Replace("Bearer ", "");
        //    var TokenInfo = _authManager.GetTokenInfo(authHeader);
        //    return TokenInfo;
        //}

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            _logger.LogInformation($"Registration Attempt for {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = _mapper.Map<User>(userDTO);
                user.UserName = userDTO.Email;
                var result = await _userManager.CreateAsync(user, userDTO.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                await _userManager.AddToRolesAsync(user, userDTO.Roles);
                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(Register)}");
                return Problem($"Something Went Wrong in the {nameof(Register)}", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO userDTO)
        {
            _logger.LogInformation($"Registration Attempt for {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (!await _authManager.ValidateUser(userDTO))
                {
                    return Unauthorized();
                }

                return Accepted(new { Token = await _authManager.CreateToken() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(Login)}");
                return Problem($"Something Went Wrong in the {nameof(Login)}", statusCode: 500);
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> Token()
        //{
        //    try
        //    {
        //        string authHeader = Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization];
        //        authHeader = authHeader.Replace("Bearer ", "");
        //        var TokenInfo = _authManager.GetTokenInfo(authHeader);

        //        var newProfileUser = await _userManager.FindByEmailAsync(TokenInfo);
        //        return Accepted(new { UserInfo = newProfileUser });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Something Went Wrong in the {nameof(Token)}");
        //        return Problem($"Something Went Wrong in the {nameof(Token)}", statusCode: 500);
        //    }

        //}

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProfile(string topic, [FromBody] ProfileUserDTO profileUserDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string authHeader = Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization];
                authHeader = authHeader.Replace("Bearer ", "");
                var TokenInfo = _authManager.GetTokenInfo(authHeader);

                var newProfileUser = await _userManager.FindByEmailAsync(TokenInfo);

                newProfileUser.FirstName = profileUserDTO.FirstName;
                newProfileUser.LastName = profileUserDTO.LastName;
                await _userManager.UpdateAsync(newProfileUser);
                await _unitOfWork.Save();

                string serializedProfileUser = JsonConvert.SerializeObject(newProfileUser);
                using (var producer = new ProducerBuilder<Null, string>(_config).Build())
                {
                    await producer.ProduceAsync(topic, new Message<Null, string> { Value = serializedProfileUser });
                    await producer.ProduceAsync(topic, new Message<Null, string> { Value = $"Update Profile User {newProfileUser.Id} successfully" });
                    producer.Flush(TimeSpan.FromSeconds(10));
                };
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(UpdateProfile)}");
                return Problem($"Something Went Wrong in the {nameof(UpdateProfile)}", statusCode: 500);
            }
        }
    }
}
