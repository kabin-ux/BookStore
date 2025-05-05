using BookStore.Entities;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookStore.DTO;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Member")]
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
        public async Task<IActionResult> AddToCart(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new BaseResponse<string>(401, false, "Unauthorized"));

            try
            {
                await _cartService.AddCartAsync(user, bookId);
                return Ok(new BaseResponse<object>(200, true, "Book added to cart.", new { bookId }));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<string>(400, false, ex.Message));
            }
        }

        [HttpPost("addQuantity")]
        public async Task<IActionResult> AddToCartQuantity([FromQuery] int bookId, [FromQuery] int quantity = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new BaseResponse<string>(401, false, "Unauthorized"));

            try
            {
                await _cartService.UpdateCartAsync(user, bookId, quantity);
                return Ok(new BaseResponse<object>(200, true, "Quantity updated in cart.", new { bookId, quantity }));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<string>(400, false, ex.Message));
            }
        }

        [HttpGet("my-cart")]
        public async Task<IActionResult> GetMyCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new BaseResponse<string>(401, false, "Unauthorized"));

            var cartItems = await _cartService.GetMyCartAsync(user);
            return Ok(new BaseResponse<object>(200, true, "Cart retrieved successfully.", cartItems));
        }

        [HttpDelete("remove/{bookId}")]
        public async Task<IActionResult> RemoveFromCart(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new BaseResponse<string>(401, false, "Unauthorized"));

            try
            {
                await _cartService.RemoveFromCartAsync(user, bookId);
                return Ok(new BaseResponse<object>(200, true, "One quantity removed or book removed from cart.", new { bookId }));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<string>(400, false, ex.Message));
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new BaseResponse<string>(401, false, "Unauthorized"));

            await _cartService.ClearCartAsync(user);
            return Ok(new BaseResponse<string>(200, true, "Cart cleared successfully."));
        }
    }
}