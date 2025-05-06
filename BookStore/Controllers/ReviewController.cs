using BookStore.DTO;
using BookStore.Entities;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class ReviewController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly IReviewService _reviewService;

        public ReviewController(UserManager<Users> userManager,IReviewService reviewService)
        {
            _userManager = userManager;
            _reviewService = reviewService;
        }

        [HttpGet("has-purchased/{bookId}")]
        public async Task<IActionResult> HasUserPurchasedBook(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            long userId = user.Id;
            var hasPurchased = await _reviewService.HasUserPurchasedBook(userId, bookId);
            if (hasPurchased)
            {
                return Ok(new BaseResponse<String>(200,true,"User has purchased the book."));
            }
            else
            {
                return BadRequest(new BaseResponse<String>(400,false,"User has not purchased this book."));
            }
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddReview([FromBody] AddReviewDTO reviewDto)
        {
            var user = await _userManager.GetUserAsync(User);
            long userId = user.Id;
            try
            {
                var result = await _reviewService.AddReview(userId, reviewDto);
                    return Ok(new BaseResponse<String>(200, true, "Added Book",result));

            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<String>(400, false, ex.Message));
            }

        }
        [HttpPut("Edit-Review/{reviewId}")]
        public async Task<IActionResult> EditReview([FromBody] AddReviewDTO reviewDto, int reviewId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                long userId = user.Id;
                var result = await _reviewService.EditReview(userId, reviewDto, reviewId);

                    return Ok(new BaseResponse<String>(200, true, result));

            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<String>(400, false, ex.Message));
            }
        }
        [HttpGet("Get-Book/{bookId}")]
        public async Task<IActionResult> GetReviewsByBook(int bookId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByBook(bookId);
                return Ok(new BaseResponse<List<GetReviewDTO>>(200, true, "Reviews fetched successfully", reviews));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<String>(400, false, $"Error: {ex.Message}"));
            }
        }

        [HttpDelete("Delete-Book/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                long userId = user.Id;
                var result = await _reviewService.DeleteReview(userId, reviewId);
                    return Ok(new BaseResponse<String>(200, true, result));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<String>(400, false,ex.Message));
            }
        }

    }
}
