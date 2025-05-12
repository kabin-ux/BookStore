using BookStore.Entities;
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

        public async Task AddOneToCartAsync(Users user, int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) throw new Exception("Book not found");

            var cartEntry = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == user.Id && c.BookId == bookId);

            if (cartEntry != null)
            {
                cartEntry.Quantity += 1;
                _context.Carts.Update(cartEntry);
            }
            else
            {
                var cart = new Carts
                {
                    UserId = user.Id,
                    BookId = bookId,
                    Quantity = 1
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

        public async Task<bool> RemoveOneItemFromCartAsync(Users user, int bookId)
        {
            var entry = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == user.Id && c.BookId == bookId);
            if (entry == null) throw new Exception("Book not in cart");

            if (entry.Quantity > 1)
            {
                entry.Quantity -= 1;
                _context.Carts.Update(entry);
            }
            else
            {
                _context.Carts.Remove(entry);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task AddCartAsync(Users user, int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) throw new Exception("Book not found");

            var cartbook = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == user.Id && c.BookId == bookId);

            if (cartbook != null)
            {
                cartbook.Quantity += 1;
                _context.Carts.Update(cartbook);
            }
            else
            {
                var cart = new Carts
                {
                    UserId = user.Id,
                    BookId = bookId,
                    Quantity = 1
                };
                await _context.Carts.AddAsync(cart);
            }

            await _context.SaveChangesAsync();
        }


        public async Task ClearCartAsync(Users user)
        {
            var entries = await _context.Carts.Where(c => c.UserId == user.Id).ToListAsync();
            _context.Carts.RemoveRange(entries);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> RemoveQuantityFromCartAsync(long userId, int bookId, int quantityToRemove)
        {
            var entry = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

            if (entry == null)
            {
                throw new Exception("Book not in cart");
            }
            if (quantityToRemove > entry.Quantity)
            {
                throw new Exception("Quantity to remove is greater than the quantity in the cart");
            }

            if (entry.Quantity > quantityToRemove)
            {
                entry.Quantity -= quantityToRemove;
                _context.Carts.Update(entry);
            }
            else
            {
                _context.Carts.Remove(entry);
            }

            await _context.SaveChangesAsync();
            return true;
        }

    }
}