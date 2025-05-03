using BookStore.Entities;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class WhitelistController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly IWhitelistService _whitelistService;

        public WhitelistController(UserManager<Users> userManager, IWhitelistService whitelistService)
        {
            _userManager = userManager;
            _whitelistService = whitelistService;
        }

        [HttpGet("debug")]
        public IActionResult DebugClaims()
        {
            return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
        }

        [HttpPost("add/{bookId}")]
        public async Task<IActionResult> AddToWhitelist(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            try
            {
                await _whitelistService.AddToWhitelistAsync(user, bookId);
                return Ok("Book added to whitelist.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-whitelist")]
        public async Task<IActionResult> GetMyWhitelist()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var list = await _whitelistService.GetMyWhitelistAsync(user);
            return Ok(list);
        }
        [HttpDelete("remove/{bookId}")]
        public async Task<IActionResult> RemoveFromWhitelist(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            try
            {
                await _whitelistService.RemoveFromWhitelistAsync(user, bookId);
                return Ok("Book removed from whitelist.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
