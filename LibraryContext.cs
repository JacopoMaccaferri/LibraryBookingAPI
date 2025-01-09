

using Microsoft.EntityFrameworkCore;

public class LibraryContext : DbContext
{
    // Constructor that initializes the DbContext with the provided options
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

    // DbSet representing the collection of books in the database
    public DbSet<Book> Books { get; set; } 

    // DbSet representing the collection of reservations in the database
    public DbSet<Reservation> Reservations { get; set; } 

    // DbSet representing the collection of customers in the database
    public DbSet<Customer> Customers { get; set; }
}
