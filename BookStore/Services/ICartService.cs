using BookStore.Entities;

namespace BookStore.Services
{
    public interface ICartService
    {
        Task UpdateCartAsync(Users user, int bookId, int quantity);
        Task<List<object>> GetMyCartAsync(Users user);
        Task<bool> RemoveFromCartAsync(Users user, int bookId);
        Task AddCartAsync(Users user, int bookId);
        Task ClearCartAsync(Users user);
        Task<bool> RemoveQuantityFromCartAsync(long user, int bookId, int quantityToRemove);
    }
}