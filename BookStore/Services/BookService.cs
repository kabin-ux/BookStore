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


        public class PagedResult<T>
        {
            public List<T> Items { get; set; }
            public int TotalPages { get; set; }
            public int TotalCount { get; set; }
        }
        public async Task<PagedResult<Books>> SearchBooks(
    string search, string sort, string author, string? genre, int pageNumber, int pageSize)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(lowerSearch) ||
                    b.Genre.ToLower().Contains(lowerSearch));
            }

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.Author.ToLower().Contains(author.ToLower()));
            }

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(b => b.Genre.ToLower().Contains(genre.ToLower()));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "title_desc":
                        query = query.OrderByDescending(b => b.Title);
                        break;
                    case "title_asc":
                        query = query.OrderBy(b => b.Title);
                        break;
                    case "price_asc":
                        query = query.OrderBy(b => b.Price);
                        break;
                    case "price_desc":
                        query = query.OrderByDescending(b => b.Price);
                        break;
                }
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Books>
            {
                Items = items,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<Books> AddBook(BookCreateUpdateDTO bookDTO)
        {
            string? imagePath = null;

            if (bookDTO.Image != null && bookDTO.Image.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "books");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bookDTO.Image.FileName);
                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await bookDTO.Image.CopyToAsync(stream);
                }

                imagePath = $"/images/books/{fileName}";
            }

            var newBook = new Books
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
                IsAvailable = bookDTO.IsAvailable,
                PublicationDate = bookDTO.PublicationDate.Date.AddHours(12), 
                ImagePath = imagePath
            };

            try
            {
                await _context.Books.AddAsync(newBook);
                await _context.SaveChangesAsync();
                return newBook;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while adding book: {ex.Message}");
                return null;
            }
        }

        public async Task<Books> UpdateBook(int id, BookCreateUpdateDTO bookDTO)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return null;
            }

            string? imagePath = book.ImagePath;

            if (bookDTO.Image != null && bookDTO.Image.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "books");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid() + Path.GetExtension(bookDTO.Image.FileName);
                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await bookDTO.Image.CopyToAsync(stream);
                }

                imagePath = $"/images/books/{fileName}";
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
            book.PublicationDate = bookDTO.PublicationDate.Date.AddHours(12);
            book.ImagePath = imagePath;

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
