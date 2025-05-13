using BookStore.DTO;
using BookStore.Entities;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtTokenService _jwtTokenService;


        public UserController(IUserService userService, JwtTokenService jwtTokenService)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register-user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterUser(UserRegisterDTO user)
        {

            var isUserExist = await _userService.FindUser(user.Email);

            if (isUserExist == false)
            {
                var isUserCreated = await _userService.AddUser(user);
                if (isUserCreated == false)
                {
                    return BadRequest("unable to create a user.");
                }
                else
                {
                    return Ok((new BaseResponse<Object>(200, true, "User created successfully")));
                }
            }
            else
            {
                return BadRequest("User already exist.");
            }
        }

        [HttpPost("public-register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterPublicUser(UserRegisterDTO user)
        {
            var isUserExist = await _userService.FindUser(user.Email);
            if (isUserExist) return BadRequest(new BaseResponse<Books>(400, false, "User with this email already exists.", null));

            var isContactTaken = await _userService.IsContactNumberTaken(user.ContactNumber);
            if (isContactTaken) return BadRequest(new BaseResponse<Books>(400, false, "User with this contact number already exists.", null));

            var isUserCreated = await _userService.AddUser(user);
            if (!isUserCreated) return BadRequest(new BaseResponse<Books>(400, false, "Unable to create user.", null)); 

            return Ok(new BaseResponse<object>(200, true, "User created successfully"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginCredential)
        {
            var (token, user, roles) = await _userService.UserLoginWithUserData(loginCredential);

            if (token != null && user != null)
            {
                return Ok(new
                {
                    message = "Login successful.",
                    token = token,
                    user = new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        user.FirstName,
                        user.LastName,
                        user.ContactNumber,
                        user.MembershipId,
                        roles // pass roles array here
                    }
                });
            }

            return Unauthorized("Invalid email or password.");
        }

        [HttpGet("all")]
        [Authorize(Roles = "Staff, Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }


        [HttpGet("ping")]
        public IActionResult Ping() => Ok("User controller is reachable.");


        [Authorize(Roles = "User")]
        [HttpGet]
        [Route("user-only")]
        public IActionResult UserOnly() => Ok(new { message = "Hello User" });


    }
}
