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
            return Ok(new BaseResponse<List<Books>>(200, true, "Books fetched successfully", books));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Books>> GetBookById(int id)
        {
            var book = await _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound(new BaseResponse<Books>(404, false, "Book not found", null));
            }

            return Ok(new BaseResponse<Books>(200, true, "Book fetched successfully", book));
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetBooks([FromQuery] BookSearchParams queryParams)
        {
            var result = await _bookService.SearchBooks(queryParams);
            //return Ok(result);

            if (result != null)
            {
                return Ok((new BaseResponse<Object>(200, true, "Books fetched successfully", result)));
            }
            else
            {
                return BadRequest(new BaseResponse<object>(400, false, "Unable to fetch books", null));
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddBook([FromForm] BookCreateUpdateDTO bookDTO)
        {
            var book = await _bookService.AddBook(bookDTO);
            if (book == null)
            {
                return BadRequest(new BaseResponse<Books>(400, false, "Failed to add book", null));
            }

            return Ok(new BaseResponse<Books>(200, true, "Book added successfully", book));
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateBook(int id, [FromForm] BookCreateUpdateDTO bookDTO)
        {
            var updatedBook = await _bookService.UpdateBook(id, bookDTO);
            if (updatedBook == null)
            {
                return NotFound(new BaseResponse<Books>(404, false, "Book not found", null));
            }

            return Ok(new BaseResponse<Books>(200, true, "Book updated successfully", updatedBook));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var deleted = await _bookService.DeleteBook(id);
            if (!deleted)
            {
                return NotFound(new BaseResponse<string>(404, false, "Book not found", null));
            }

            return Ok(new BaseResponse<string>(200, true, "Book deleted successfully", null));
        }

        [HttpGet("authors")]
        public async Task<IActionResult> GetAuthors()
        {
            var authors = await _bookService.GetUniqueAuthorsAsync();
            var result = authors.Select(a => new { label = a, value = a }).ToList<object>();

            return Ok(new BaseResponse<List<object>>(200, true, "Authors fetched successfully", result));
        }
    }
}
