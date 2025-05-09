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
        public async Task<IActionResult> RegisterUserByAdmin(UserRegisterDTO user)
        {
            var isUserExist = await _userService.FindUser(user.Email);
            if (isUserExist) return BadRequest("User already exists.");

            var isUserCreated = await _userService.AddUser(user, "Admin");
            if (!isUserCreated) return BadRequest("Unable to create user.");

            return Ok(new BaseResponse<object>(200, true, "User created successfully"));
        }


        [HttpPost("public-register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterPublicUser(UserRegisterDTO user)
        {
            var isUserExist = await _userService.FindUser(user.Email);
            if (isUserExist) return BadRequest("User already exists.");

            // Role is not passed intentionally
            var isUserCreated = await _userService.AddUser(user);
            if (!isUserCreated) return BadRequest("Unable to create user.");

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


        [HttpGet("ping")]
        public IActionResult Ping() => Ok("User controller is reachable.");


        [Authorize(Roles = "User")]
        [HttpGet]
        [Route("user-only")]
        public IActionResult UserOnly() => Ok(new { message = "Hello User" });


    }
}

