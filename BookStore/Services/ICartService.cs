using BookStore.Entities;

namespace BookStore.Services
{
    public interface ICartService
    {
        Task AddOrUpdateCartAsync(Users user, int bookId, int quantity);
        Task<List<object>> GetMyCartAsync(Users user);
        Task<bool> RemoveFromCartAsync(Users user, int bookId, int quantity);
    }
}
