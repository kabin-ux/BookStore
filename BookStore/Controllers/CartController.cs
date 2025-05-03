using BookStore.Entities;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class CartController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly ICartService _cartService;

        public CartController(UserManager<Users> userManager, ICartService cartService)
        {
            _userManager = userManager;
            _cartService = cartService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromQuery] int bookId, [FromQuery] int quantity = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            try
            {
                await _cartService.AddOrUpdateCartAsync(user, bookId, quantity);
                return Ok("Book added/updated in cart.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-cart")]
        public async Task<IActionResult> GetMyCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var list = await _cartService.GetMyCartAsync(user);
            return Ok(list);
        }

        [HttpDelete("remove/{bookId}")]
        public async Task<IActionResult> RemoveFromCart(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            try
            {
                await _cartService.RemoveFromCartAsync(user, bookId);
                return Ok("Book removed from cart.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}