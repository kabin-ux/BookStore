using BookStore.DTO;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register-user")]
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
                    return Ok("User created successfully.");
                }
            }
            else
            {
                return BadRequest("User already exist.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginCredential)
        {
            var isLoggedIn = await _userService.UserLogin(loginCredential);


            if (isLoggedIn)
            {
                return Ok("Login successful.");
            }

            return Unauthorized("Invalid email or password.");
        }
    }
}

  
