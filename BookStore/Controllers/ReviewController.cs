using BookStore.DTO;
using BookStore.Entities;
using BookStore.Exceptions;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly IReviewService _reviewService;

        public ReviewController(UserManager<Users> userManager, IReviewService reviewService)
        {
            _userManager = userManager;
            _reviewService = reviewService;
        }

        [HttpGet("has-purchased/{bookId}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> HasUserPurchasedBook(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            long userId = user.Id;
            var hasPurchased = await _reviewService.HasUserPurchasedBook(userId, bookId);
            if (hasPurchased)
            {
                return Ok(new BaseResponse<String>(200, true, "User has purchased the book."));
            }
            else
            {
                throw new ForbiddenException("User has not purchased this book.");
            }
        }
        [HttpPost("add")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddReview([FromBody] AddReviewDTO reviewDto)
        {
            var user = await _userManager.GetUserAsync(User);
            long userId = user.Id;
            var result = await _reviewService.AddReview(userId, reviewDto);
            return Ok(new BaseResponse<String>(200, true, "Added Book", result));

        }
        [HttpPut("Edit-Review/{reviewId}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> EditReview([FromBody] AddReviewDTO reviewDto, int reviewId)
        {
            var user = await _userManager.GetUserAsync(User);
            long userId = user.Id;
            var result = await _reviewService.EditReview(userId, reviewDto, reviewId);

            return Ok(new BaseResponse<String>(200, true, result));


        }
        [HttpGet("Get-Book/{bookId}")]
        public async Task<IActionResult> GetReviewsByBook(int bookId)
        {

            var reviews = await _reviewService.GetReviewsByBook(bookId);
            return Ok(new BaseResponse<List<GetReviewDTO>>(200, true, "Reviews fetched successfully", reviews));

        }

        [HttpDelete("Delete-Book/{reviewId}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var user = await _userManager.GetUserAsync(User);
            long userId = user.Id;
            var result = await _reviewService.DeleteReview(userId, reviewId);
            return Ok(new BaseResponse<String>(200, true, result));

        }
    }
}
