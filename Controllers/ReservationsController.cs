using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(LibraryContext context, ILogger<ReservationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/Reservations
        /// <summary>
        /// Adds a new reservation.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <param name="bookId">The ID of the book.</param>
        /// <response code="201">Returns the newly created reservation</response>
        /// <response code="400">If the book is not available or the customer is not found</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Reservation>> AddReservation(int customerId, int bookId)
        {
            _logger.LogInformation("Adding reservation for customer {CustomerId} and book {BookId}", customerId, bookId);

            var book = await _context.Books.FindAsync(bookId);
            if (book == null || book.Status != BookStatus.Available)
            {
                _logger.LogWarning("Book {BookId} not available for reservation", bookId);
                return BadRequest("Il libro non è disponibile per la prenotazione.");
            }

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                _logger.LogWarning("Customer {CustomerId} not found", customerId);
                return BadRequest("Cliente non trovato.");
            }

            var existingReservation = await _context.Reservations
                .Where(r => r.CustomerId == customerId && r.BookId == bookId)
                .FirstOrDefaultAsync();

            if (existingReservation != null)
            {
                _logger.LogWarning("Customer {CustomerId} already reserved book {BookId}", customerId, bookId);
                return BadRequest("Il cliente ha già prenotato una copia di questo libro.");
            }

            var reservation = new Reservation
            {
                CustomerId = customerId,
                BookId = bookId,
                ReservationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(7)
            };

            _context.Entry(book).State = EntityState.Modified;
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Reservation for customer {CustomerId} and book {BookId} created successfully", customerId, bookId);

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
        }

        // GET: api/Reservations/5
        /// <summary>
        /// Retrieves a reservation by its ID.
        /// </summary>
        /// <param name="id">The ID of the reservation.</param>
        /// <response code="200">Returns the reservation</response>
        /// <response code="404">If the reservation is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            _logger.LogInformation("Retrieving reservation with ID {ReservationId}", id);

            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                _logger.LogWarning("Reservation with ID {ReservationId} not found", id);
                return NotFound();
            }

            return reservation;
        }

        // DELETE: api/Reservations/5
        /// <summary>
        /// Deletes a reservation.
        /// </summary>
        /// <param name="id">The ID of the reservation to delete.</param>
        /// <response code="204">If the deletion is successful</response>
        /// <response code="404">If the reservation is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            _logger.LogInformation("Deleting reservation with ID {ReservationId}", id);

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                _logger.LogWarning("Reservation with ID {ReservationId} not found", id);
                return NotFound();
            }

            var book = await _context.Books.FindAsync(reservation.BookId);
            if (book != null)
            {
                book.Status = BookStatus.Available;
                _context.Entry(book).State = EntityState.Modified;
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Reservation with ID {ReservationId} deleted successfully", id);

            return NoContent();
        }
    }
}
