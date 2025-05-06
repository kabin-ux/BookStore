using BookStore.DTO;

namespace BookStore.Services
{
    public interface IReviewService
    {

        Task<bool> HasUserPurchasedBook(long userId, int bookId);

      
        Task<string> AddReview(long userId, AddReviewDTO reviewDto);

        Task<string> EditReview(long userId, AddReviewDTO reviewDto,long reviewId);

        Task<List<GetReviewDTO>> GetReviewsByBook(int bookId);

        Task<string> DeleteReview(long userId, int reviewId);
    }
}
