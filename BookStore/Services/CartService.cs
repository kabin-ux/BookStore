using BookStore.Entities;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDBContext _context;

        public CartService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task AddOrUpdateCartAsync(Users user, int bookId, int quantity)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) throw new Exception("Book not found");

            var cartEntry = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.BookId == bookId);

            if (cartEntry != null)
            {
                cartEntry.Quantity += quantity;
                _context.Carts.Update(cartEntry);
            }
            else
            {
                var cart = new Carts
                {
                    UserId = user.Id,
                    BookId = bookId,
                    Quantity = quantity
                };
                await _context.Carts.AddAsync(cart);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<object>> GetMyCartAsync(Users user)
        {
            var cartItems = await _context.Carts
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Book)
                .Select(c => new
                {
                    c.CartId,
                    c.Book.BookId,
                    c.Book.Title,
                    c.Book.Author,
                    c.Quantity,
                    c.Book.Price
                }).ToListAsync();

            return cartItems.Cast<object>().ToList();
        }

        public async Task<bool> RemoveFromCartAsync(Users user, int bookId)
        {
            var entry = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == user.Id && c.BookId == bookId);
            if (entry == null) throw new Exception("Book not in cart");

            _context.Carts.Remove(entry);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}