using BookStore.DTO;
using BookStore.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDBContext _context;

        public BookService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<Books>> GetBooks(int pageNumber, int pageSize)
        {
            return await _context.Books
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Books> GetBookById(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<List<Books>> SearchBooks(string search, string sort, string author, int? year)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Title.Contains(search) || b.Genre.Contains(search));
            }

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.Author.Contains(author));
            }

            if (year.HasValue)
            {
                // You must have a PublishedYear property in your model for this. (currently not in Books)
                // Example: query = query.Where(b => b.PublishedYear == year.Value);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                if (sort.ToLower() == "title_desc")
                    query = query.OrderByDescending(b => b.Title);
                else if (sort.ToLower() == "title_asc")
                    query = query.OrderBy(b => b.Title);
                // add more sorting logic if needed
            }

            return await query.ToListAsync();
        }

        public async Task<Books> AddBook(BookCreateUpdateDTO bookDTO)
        {
            var book = new Books
            {
                Title = bookDTO.Title,
                Author = bookDTO.Author,
                Genre = bookDTO.Genre,
                Language = bookDTO.Language,
                Publisher = bookDTO.Publisher,
                Format = bookDTO.Format,
                ISBN = bookDTO.ISBN,
                StockQuantity = bookDTO.StockQuantity,
                Price = bookDTO.Price,
                IsAvailable = bookDTO.IsAvailable
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Books> UpdateBook(int id, BookCreateUpdateDTO bookDTO)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return null;
            }

            book.Title = bookDTO.Title;
            book.Author = bookDTO.Author;
            book.Genre = bookDTO.Genre;
            book.Language = bookDTO.Language;
            book.Publisher = bookDTO.Publisher;
            book.Format = bookDTO.Format;
            book.ISBN = bookDTO.ISBN;
            book.StockQuantity = bookDTO.StockQuantity;
            book.Price = bookDTO.Price;
            book.IsAvailable = bookDTO.IsAvailable;

            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return false;
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
