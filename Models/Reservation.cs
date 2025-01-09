public class Reservation
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int BookId { get; set; }
    public DateTime? ReservationDate { get; set; }
    public DateTime? ExpirationDate { get; set; }

    // Navigational properties
    public Customer? Customer { get; set; }
    public Book? Book { get; set; }
}
