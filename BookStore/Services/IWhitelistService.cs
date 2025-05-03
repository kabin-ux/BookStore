using BookStore.Entities;

namespace BookStore.Services
{
    public interface IWhitelistService
    {
        Task<bool> AddToWhitelistAsync(Users user, int bookId);
        Task<List<object>> GetMyWhitelistAsync(Users user);
        Task<bool> RemoveFromWhitelistAsync(Users user, int bookId);

    }
}
