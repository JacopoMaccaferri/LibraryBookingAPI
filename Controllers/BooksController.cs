using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly ILogger<BooksController> _logger;

        public BooksController(LibraryContext context, ILogger<BooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Books
        /// <summary>
        /// Retrieves a list of books.
        /// </summary>
        /// <response code="200">Returns the list of books</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            _logger.LogInformation("Retrieving list of books.");
            return await _context.Books.ToListAsync();
        }

        // GET: api/Books/5
        /// <summary>
        /// Retrieves a book by its ID.
        /// </summary>
        /// <param name="id">The ID of the book.</param>
        /// <response code="200">Returns the book</response>
        /// <response code="404">If the book is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            _logger.LogInformation("Retrieving book with ID {BookId}", id);
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                _logger.LogWarning("Book with ID {BookId} not found", id);
                return NotFound();
            }

            return book;
        }

        // POST: api/Books
        /// <summary>
        /// Creates a new book.
        /// </summary>
        /// <param name="book">The book to create.</param>
        /// <response code="201">Returns the newly created book</response>
        /// <response code="400">If the book is null</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            _logger.LogInformation("Creating a new book.");

            if (book == null)
            {
                _logger.LogWarning("Book data is null.");
                return BadRequest();
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Book created with ID {BookId}", book.Id);

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // PUT: api/Books/5
        /// <summary>
        /// Updates an existing book.
        /// </summary>
        /// <param name="id">The ID of the book.</param>
        /// <param name="book">The updated book object.</param>
        /// <response code="204">If the update is successful</response>
        /// <response code="400">If the book ID does not match</response>
        /// <response code="404">If the book is not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                _logger.LogWarning("Book ID in URL ({BookId}) does not match book ID in body ({BodyBookId}).", id, book.Id);
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    _logger.LogWarning("Book with ID {BookId} not found.", id);
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            _logger.LogInformation("Book with ID {BookId} updated successfully.", id);

            return NoContent();
        }

        // DELETE: api/Books/5
        /// <summary>
        /// Deletes a book.
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <response code="204">If the deletion is successful</response>
        /// <response code="404">If the book is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            _logger.LogInformation("Deleting book with ID {BookId}.", id);

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                _logger.LogWarning("Book with ID {BookId} not found.", id);
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Book with ID {BookId} deleted successfully.", id);

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        // SEARCH: api/Books/search?title=someTitle&author=someAuthor&status=Available
        /// <summary>
        /// Searches for books by title, author, and/or status.
        /// </summary>
        /// <param name="title">The title of the book(s) to search for.</param>
        /// <param name="author">The author of the book(s) to search for.</param>
        /// <param name="status">The status of the book(s) to search for.</param>
        /// <response code="200">Returns the list of books matching the search criteria</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBooks([FromQuery] string? title, [FromQuery] string? author, [FromQuery] BookStatus? status)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                _logger.LogInformation("Filtering books by title: {Title}", title);
                query = query.Where(b => b.Title.Contains(title));
            }

            if (!string.IsNullOrEmpty(author))
            {
                _logger.LogInformation("Filtering books by author: {Author}", author);
                query = query.Where(b => b.Author.Contains(author));
            }

            if (status.HasValue)
            {
                _logger.LogInformation("Filtering books by status: {Status}", status);
                query = query.Where(b => b.Status == status.Value);
            }

            return await query.ToListAsync();
        }
    }
}


