using BookStore.DTO;
using BookStore.DTOs;
using BookStore.Entities;
using static BookStore.Services.BookService;

namespace BookStore.Services
{
    public interface IBookService
    {
        Task<List<Books>> GetBooks(int pageNumber, int pageSize);
        Task<BookWithDiscountDTO?> GetBookById(int id);
        Task<PagedResult<BookWithDiscountDTO>> SearchBooks(BookSearchParams filters);
        Task<Books> AddBook(BookCreateUpdateDTO bookDTO);
        Task<Books> UpdateBook(int id, BookCreateUpdateDTO bookDTO);
        Task<bool> DeleteBook(int id);
        Task<List<string>> GetUniqueAuthorsAsync();

    }
}
