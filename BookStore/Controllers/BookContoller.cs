using BookStore.DTO;
using BookStore.Entities;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;
using static BookStore.Services.BookService;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Books>>> GetBooks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var books = await _bookService.GetBooks(pageNumber, pageSize);
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Books>> GetBookById(int id)
        {
            var book = await _bookService.GetBookById(id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<Books>>> SearchBooks(
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] string? author,
            [FromQuery] string? genre,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _bookService.SearchBooks(search, sort, author, genre, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddBook([FromForm] BookCreateUpdateDTO bookDTO)
        {
            var book = await _bookService.AddBook(bookDTO);
            if (book == null)
            {
                return BadRequest(new
                {
                    message = "Failed to add book",
                    success = false
                });
            }

            return Ok(new
            {
                message = "Book added successfully",
                success = true,
                data = book
            });
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateBook(int id, [FromForm] BookCreateUpdateDTO bookDTO)
        {
            var updatedBook = await _bookService.UpdateBook(id, bookDTO);
            if (updatedBook == null)
            {
                return NotFound(new
                {
                    message = "Book not found",
                    success = false
                });
            }

            return Ok(new
            {
                message = "Book updated successfully",
                success = true,
                data = updatedBook
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var deleted = await _bookService.DeleteBook(id);
            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Book not found",
                    success = false
                });
            }

            return Ok(new
            {
                message = "Book deleted successfully",
                success = true
            });
        }
    }
}