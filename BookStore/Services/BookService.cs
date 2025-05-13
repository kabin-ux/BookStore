using BookStore.DTO;
using BookStore.DTOs;
using BookStore.Entities;
using BookStore.Helper;
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

        public async Task<BookWithDiscountDTO?> GetBookById(int id)
        {
            var book = await _context.Books
                .Include(b => b.Discounts)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
                return null;

            var activeDiscount = DiscountHelper.GetActiveDiscount(book.Discounts);

            return new BookWithDiscountDTO
            {
                BookId = book.BookId,
                Title = book.Title,
                Description = book.Description,
                Author = book.Author,
                Genre = book.Genre,
                Language = book.Language,
                Publisher = book.Publisher,
                Format = book.Format,
                ISBN = book.ISBN,
                StockQuantity = book.StockQuantity,
                Price = book.Price,
                IsAvailable = book.IsAvailable,
                IsStoreOnlyAccess = book.IsStoreOnlyAccess,
                ImagePath = book.ImagePath,
                PublicationDate = book.PublicationDate,
                ArrivalDate = book.ArrivalDate,
                ActiveDiscount = activeDiscount == null ? null : new DiscountDTO
                {
                    DiscountPercent = activeDiscount.DiscountPercent,
                    DiscountedPrice = activeDiscount.DiscountedPrice,
                    StartDate = activeDiscount.StartDate,
                    EndDate = activeDiscount.EndDate,
                    IsOnSale = activeDiscount.IsOnSale,

                }
            };
        }

        public class PagedResult<T>
        {
            public List<T> Items { get; set; }
            public int TotalItems { get; set; }
            public int TotalPages { get; set; }
            public int PageSize { get; set; }
            public int CurrentItemCount { get; set; } 
        }

        public async Task<PagedResult<BookWithDiscountDTO>> SearchBooks(BookSearchParams filters)
        {
            var query = _context.Books
                .Include(b => b.Discounts)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filters.Search))
            {
                var lowerSearch = filters.Search.ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(lowerSearch) ||
                    b.ISBN.ToLower().Contains(lowerSearch));
            }

            if (!string.IsNullOrEmpty(filters.Author))
            {
                query = query.Where(b => b.Author.ToLower() == filters.Author.ToLower());
            }

            if (!string.IsNullOrEmpty(filters.Genre))
            {
                query = query.Where(b => b.Genre.ToLower().Contains(filters.Genre.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.Format))
            {
                query = query.Where(b => b.Format.ToLower().Contains(filters.Format.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.Language))
            {
                query = query.Where(b => b.Language.ToLower().Contains(filters.Language.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.Availability))
            {
                switch (filters.Availability.ToLower())
                {
                    case "in_stock":
                        query = query.Where(b => b.StockQuantity > 0);
                        break;
                    case "physical":
                        query = query.Where(b => b.IsStoreOnlyAccess);
                        break;
                }
            }

            if (decimal.TryParse(filters.MinPrice, out var minPrice))
            {
                query = query.Where(b => b.Price >= minPrice);
            }

            if (decimal.TryParse(filters.MaxPrice, out var maxPrice))
            {
                query = query.Where(b => b.Price <= maxPrice);
            }

            if (!string.IsNullOrEmpty(filters.Sort))
            {
                switch (filters.Sort.ToLower())
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
                    case "pub_date_asc":
                        query = query.OrderBy(b => b.PublicationDate);
                        break;
                    case "pub_date_desc":
                        query = query.OrderByDescending(b => b.PublicationDate);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(filters.Filter))
            {
                var now = DateTime.UtcNow;

                switch (filters.Filter.ToLower())
                {
                    case "new":
                        var threeMonthsAgo = now.AddMonths(-3);
                        query = query.Where(b => b.PublicationDate >= threeMonthsAgo);
                        break;

                    case "bestsellers":
                        var bestSellingBookIds = await _context.OrderItems
                            .Where(oi => oi.Order.Status.ToLower() == "completed")
                            .GroupBy(oi => oi.BookId)
                            .Where(g => g.Sum(oi => oi.Quantity) > 4)
                            .Select(g => g.Key)
                            .ToListAsync();

                        query = query.Where(b => bestSellingBookIds.Contains(b.BookId));
                        break;

                    case "winners":
                        query = query.Where(b => b.Description != null && b.Description.ToLower().Contains("award"));
                        break;


                    case "comingsoon":
                        query = query.Where(b => b.ArrivalDate > now);
                        break;

                    case "sale":
                        var activeDiscounts = await _context.Discounts
                            .Where(d => d.StartDate <= now && d.EndDate >= now)
                            .Select(d => d.BookId)
                            .ToListAsync();
                        query = query.Where(b => activeDiscounts.Contains(b.BookId));
                        break;

                }
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)filters.PageSize);

            var items = await query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            var itemDtos = items.Select(book =>
            {
                var activeDiscount = DiscountHelper.GetActiveDiscount(book.Discounts);
                return new BookWithDiscountDTO
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Description = book.Description,
                    Author = book.Author,
                    Genre = book.Genre,
                    Language = book.Language,
                    Publisher = book.Publisher,
                    Format = book.Format,
                    ISBN = book.ISBN,
                    StockQuantity = book.StockQuantity,
                    Price = book.Price,
                    IsAvailable = book.IsAvailable,
                    IsStoreOnlyAccess = book.IsStoreOnlyAccess,
                    ImagePath = book.ImagePath,
                    PublicationDate = book.PublicationDate,
                    ArrivalDate = book.ArrivalDate,
                    ActiveDiscount = activeDiscount == null ? null : new DiscountDTO
                    {
                        DiscountPercent = activeDiscount.DiscountPercent,
                        DiscountedPrice = activeDiscount.DiscountedPrice,
                        StartDate = activeDiscount.StartDate,
                        EndDate = activeDiscount.EndDate,
                        IsOnSale = activeDiscount.IsOnSale,
                    }
                };
            }).ToList();

            return new PagedResult<BookWithDiscountDTO>
            {
                Items = itemDtos,
                TotalItems = totalCount,
                TotalPages = totalPages,
                PageSize = filters.PageSize,
                CurrentItemCount = itemDtos.Count
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
                Description = bookDTO.Description,
                Author = bookDTO.Author,
                Genre = bookDTO.Genre,
                Language = bookDTO.Language,
                Publisher = bookDTO.Publisher,
                Format = bookDTO.Format,
                ISBN = bookDTO.ISBN,
                StockQuantity = bookDTO.StockQuantity,
                Price = bookDTO.Price,
                IsAvailable = bookDTO.IsAvailable,
                IsStoreOnlyAccess = bookDTO.IsStoreOnlyAccess,
                PublicationDate = bookDTO.PublicationDate, 
                ArrivalDate = bookDTO.ArrivalDate,
                ImagePath = imagePath,

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
            book.Description = bookDTO.Description;
            book.Author = bookDTO.Author;
            book.Genre = bookDTO.Genre;
            book.Language = bookDTO.Language;
            book.Publisher = bookDTO.Publisher;
            book.Format = bookDTO.Format;
            book.ISBN = bookDTO.ISBN;
            book.StockQuantity = bookDTO.StockQuantity;
            book.Price = bookDTO.Price;
            book.IsAvailable = bookDTO.IsAvailable;
            book.IsStoreOnlyAccess = bookDTO.IsStoreOnlyAccess;
            book.PublicationDate = bookDTO.PublicationDate.Date.AddHours(0);
            book.ArrivalDate = bookDTO.ArrivalDate.Date.AddHours(0);
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

        public async Task<List<string>> GetUniqueAuthorsAsync()
        {
            return await _context.Books
                .Where(b => !string.IsNullOrEmpty(b.Author))
                .Select(b => b.Author.Trim())
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
        }

    }
}
