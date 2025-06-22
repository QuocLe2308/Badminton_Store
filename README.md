# Badminton Store

A backend system for an e-commerce platform specializing in badminton rackets, shuttles, and related gear. Built with ASP.NET Core, this project handles the core functionalities of the online store, including order processing, inventory management, and payment verification.

## Features

- **Order Management**: Create new orders from customer shopping carts.
- **Inventory Control**: Automatically checks product stock before an order and updates quantities after a successful order.
- **Payment Verification**: Integrates with an external service to check and confirm payment status for orders.
- **RESTful API**: Provides clean and robust API endpoints for front-end integration.

## Technologies Used

- **Backend**: C#, .NET Core, ASP.NET Core
- **Data Access**: Entity Framework Core
- **Database**: Microsoft SQL Server (or any other EF Core compatible database)
- **API**: RESTful APIs
- **Architecture**: Layered Architecture (DataAccessLayer, BusinessLayer, WebApp)
- **Tooling**: Swagger, Postman, Git

## Getting Started

Follow these instructions to get a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (Version should be compatible with the project)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or another database server.
- A local web server (like XAMPP or IIS) to host the `lsgd.php` file for payment simulation.

### Installation

1.  **Clone the repository**
    ```sh
    git clone https://github.com/QuocLe2308/Badminton_Store.git
    cd Badminton_Store
    ```

2.  **Configure Database Connection**
    - Open `WebApp/appsettings.json`.
    - Modify the `ConnectionString` to point to your local database server.

3.  **Apply Database Migrations**
    - Navigate to the `WebApp` directory where the `.csproj` file is located.
    - Run the following command to create the database and apply the schema:
    ```sh
    dotnet ef database update
    ```

4.  **Set up Payment Verification Simulator**
    - The `CheckPayment` feature depends on a local PHP script (`lsgd.php`) running at `http://localhost/lsgd.php`.
    - You need to set up a local web server (like Apache via XAMPP) and place a `lsgd.php` file in its root directory. This script should return a JSON response with transaction data that the `CheckPayment` endpoint can parse.

5.  **Run the Application**
    - Navigate to the `WebApp` directory.
    - Run the project using the .NET CLI:
    ```sh
    dotnet run
    ```
    - The API will be available at `https://localhost:xxxx` or `http://localhost:yyyy` (check the console output for the exact ports). You can access the Swagger UI at `https://localhost:xxxx/swagger`.

## API Endpoints

Here are the main endpoints provided by the `CartOrderController`:

- `POST /api/CartOrder/CreateOrder`
  - Creates a new order.
  - **Body**: Requires a JSON object with `shippingInfo` and `cartItems`.
- `GET /api/CartOrder/CheckPayment`
  - Checks the payment status of an order.
  - **Parameters**: `id` (the Order ID). 
