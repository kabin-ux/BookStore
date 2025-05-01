using BookStore.DTO;
using BookStore.Entities;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<List<Books>>> SearchBooks([FromQuery] string? search, [FromQuery] string? sort, [FromQuery] string? author, [FromQuery] int? year)
        {
            var books = await _bookService.SearchBooks(search, sort, author, year);
            return Ok(books);
        }

        [HttpPost]
        public async Task<ActionResult<Books>> AddBook([FromBody] BookCreateUpdateDTO bookDTO)
        {
            var book = await _bookService.AddBook(bookDTO);
            return CreatedAtAction(nameof(GetBookById), new { id = book.BookId }, book);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Books>> UpdateBook(int id, [FromBody] BookCreateUpdateDTO bookDTO)
        {
            var updatedBook = await _bookService.UpdateBook(id, bookDTO);
            if (updatedBook == null)
                return NotFound();

            return Ok(updatedBook);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            var deleted = await _bookService.DeleteBook(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
