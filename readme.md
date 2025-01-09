# Library Booking API Overview 
The Library Booking API is a web application that allows users to manage books, customers, and reservations in a library system. It is built using ASP.NET Core and Entity Framework Core with PostgreSQL as the database.

# Prerequisites 
- .NET 6 SDK or later
- PostgreSQL database
- An IDE or text editor (e.g., Visual Studio, Visual Studio Code)
- dotnet-ef tool for Entity Framework Core migrations

# Setup

1. Clone the Repository Clone the repository to your local machine using the following command:
```bash
git clone
```

3. Configure the Database Update the connection string in the appsettings.json file with your PostgreSQL database information:
```json
{ "ConnectionStrings":{ "DefaultConnection": "Host=;Database=;Username=;Password=" } }
```

3. Apply Migrations Navigate to the project directory and apply the Entity Framework Core migrations to create the database schema:
```bash
dotnet ef database update
```
4. Run the Application Run the application using the following command:
```bash 
dotnet run
```

The API will be available at https://localhost:5089

# Endpoints

## Books
- GET /api/Books - Retrieve a list of books.

- GET /api/Books/{id} - Retrieve a book by its ID.

- POST /api/Books - Create a new book.

- PUT /api/Books/{id} - Update an existing book.

- DELETE /api/Books/{id} - Delete a book by its ID.

- GET /api/Books/search?title=someTitle&author=someAuthor&status=Available - Search for books by title, author, and/or status.

## Customers 
- GET /api/Customers - Retrieve a list of customers.

- GET /api/Customers/{id} - Retrieve a customer by their ID.

- POST /api/Customers - Create a new customer.

- PUT /api/Customers/{id} - Update an existing customer.

- DELETE /api/Customers/{id} - Delete a customer by their ID.

## Reservations POST /api/Reservations - Add a new reservation.

- GET /api/Reservations/{id} - Retrieve a reservation by its ID.

- DELETE /api/Reservations/{id} - Delete a reservation by its ID.

# Swagger 
Swagger is configured to provide API documentation. You can access the Swagger UI at https://localhost:5089 when the application is running. Swagger UI will be available at the root URL.

License This project is licensed under the MIT License. See the LICENSE file for more details.

Contributing Contributions are welcome! Please submit a pull request or open an issue to discuss changes or improvements.
