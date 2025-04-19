using BookStore.DTO;
using BookStore.Entities;

namespace BookStore.Services
{
    public interface IBookService
    {
        Task<List<Books>> GetBooks(int pageNumber, int pageSize);
        Task<Books> GetBookById(int id);
        Task<List<Books>> SearchBooks(string search, string sort, string author, int? year);
        Task<Books> AddBook(BookCreateUpdateDTO bookDTO);
        Task<Books> UpdateBook(int id, BookCreateUpdateDTO bookDTO);
        Task<bool> DeleteBook(int id);
    }
}
