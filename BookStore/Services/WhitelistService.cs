using BookStore.Entities;
using BookStore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services
{
    public class WhitelistService : IWhitelistService
    {
        private readonly ApplicationDBContext _context;

        public WhitelistService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddToWhitelistAsync(Users user, int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
                throw new NotFoundException("Book not found");

            var exists = await _context.Whitelists.AnyAsync(w => w.UserId == user.Id && w.BookId == bookId);
            if (exists)
                throw new ConflictException("Book already in whitelist");

            var whitelist = new Whitelists
            {
                UserId = user.Id,
                BookId = bookId,
                IsAvailable = AvailabilityStatus.Yes,
                DateAdded = DateTime.UtcNow
            };

            await _context.Whitelists.AddAsync(whitelist);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<object>> GetMyWhitelistAsync(Users user)
        {
            var list = await _context.Whitelists
                .Where(w => w.UserId == user.Id)
                .Include(w => w.Book)
                .Select(w => new
                {
                    w.Book.BookId,
                    w.Book.Title,
                    w.Book.Author,
                    w.IsAvailable,
                    w.Book.IsStoreOnlyAccess,
                    w.Book.StockQuantity,
                    w.DateAdded,
                    w.Book.Price,
                    w.Book.ImagePath
                }).ToListAsync();

            if (!list.Any())
                throw new NotFoundException("Whitelist is empty");

            return list.Cast<object>().ToList();
        }

        public async Task<bool> RemoveFromWhitelistAsync(Users user, int bookId)
        {
            var entry = await _context.Whitelists
                .FirstOrDefaultAsync(w => w.UserId == user.Id && w.BookId == bookId);

            if (entry == null)
                throw new NotFoundException("Book not found in whitelist");

            _context.Whitelists.Remove(entry);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}