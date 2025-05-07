using BookStore.DTO;
using BookStore.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDBContext _context;
        private readonly IOrdersService _ordersService;

        public ReviewService(ApplicationDBContext context, IOrdersService ordersService)
        {
            _context = context;
            _ordersService = ordersService;
        }
        public async Task<bool> HasUserPurchasedBook(long userId, int bookId)
        {
            var orders = await _ordersService.GetUserOrders(userId);
            return orders.Any(order => order.OrderItems.Any(item => item.BookId == bookId));
        }
        public async Task<string> AddReview(long userId, AddReviewDTO reviewDto)
        {
            var hasPurchased = await HasUserPurchasedBook(userId, reviewDto.BookId);
            if (!hasPurchased)
            {
                throw new Exception("You can only review books you've purchased.");
            }

            var review = new Reviews
            {
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                Date = DateTime.UtcNow,
                BookId = reviewDto.BookId,
                UserId = userId
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return "Review added successfully";
        }

        public async Task<string> EditReview(long userId, AddReviewDTO reviewDto, long reviewId)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == reviewId);
            if (review == null)
            {
                throw new Exception("Review not found.");
            }

            if (review.UserId != userId)
            {
                throw new Exception("You can only edit your own reviews.");
            }

            review.Rating = reviewDto.Rating;
            review.Comment = reviewDto.Comment;
            review.Date = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return "Review updated successfully";
        }
        public async Task<List<GetReviewDTO>> GetReviewsByBook(int bookId)
        {
            return await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Select(r => new GetReviewDTO
                {
                    ReviewId = r.ReviewId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    Date = r.Date
                })
                .ToListAsync();
        }
        public async Task<string> DeleteReview(long userId, int reviewId)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == reviewId);
            if (review == null)
            {
                throw new Exception("Review not found.");
            }

            if (review.UserId != userId)
            {
                throw new Exception("You can only delete your own reviews.");
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return "Review deleted successfully";
        }
    }
}