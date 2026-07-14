# SariKart API V2

A RESTful Web API built with ASP.NET Core 6.0 and Entity Framework Core for a grocery/sari-sari store ordering and delivery management system.

## Tech Stack

- **Framework**: ASP.NET Core 6.0
- **ORM**: Entity Framework Core 6.0.36
- **Database**: Microsoft SQL Server
- **API Docs**: Swashbuckle (Swagger) 6.5.0
- **Architecture**: RESTful API with Controller-based routing

## Database

Hosted on: `KiNoDBFightingMachine.mssql.somee.com`

### Schema Overview

| Table | Description |
|-------|-------------|
| `AppUser` | System users (Customers, Admins, Riders, Cashiers) |
| `UserType` | Lookup table for user roles |
| `Category` | Product categories with icons |
| `Product` | Inventory items with pricing and stock |
| `ShopOrder` | Customer orders with payment details |
| `OrderLine` | Line items within an order |
| `Delivery` | Order delivery assignments |
| `StoreBranch` | Physical store locations |
| `UserStore` | User-specific store addresses |
| `OrderStatus` | Order status lifecycle |
| `Vehicle` | Delivery vehicle types |

### Seed Data

- **User Types**: Customer, Admin, Rider, Cashier
- **Order Statuses**: Pending, Processing, Cancelled, Followed Up
- **Vehicles**: Motorcycle
- **Store Branch**: Main Branch
- **Default Users**: admin / admin, rider / rider, cashier / cashier
- **Categories**: Rice & Grains, Canned Goods, Snacks, Beverages, Vegetables, Condiments
- **Products**: 12 seeded products with pricing, stock, and images

## Features

### User Management
- Register and manage users (Customers, Admins, Riders, Cashiers)
- Login authentication
- Editable contact information per user
- User-specific store addresses

### Product & Category Management
- Full CRUD for categories with icon upload support (`IFormFile`)
- Full CRUD for products
- Stock tracking
- Price and unit management
- Product categorization

### Order Management
- Create orders with multiple line items
- Order status lifecycle (Pending, Processing, Cancelled, Followed Up)
- Order search and filtering by customer name and status
- Order contact details (person, number, address)
- Payment tracking (Total Amount, Amount Paid, Change)

### Delivery Management
- Assign riders to orders
- Store branch selection for order pickup
- Vehicle assignment per delivery
- Delivery date tracking
- Cash on hand flag
- Delivery completion status

### Store Management
- Multiple store branches with address details
- User-defined store addresses

### Search & Filtering
- Search orders by customer name
- Filter orders by order status

### API Documentation
- Swagger UI available in Development environment at `/swagger`

## UI & Design

### Frontend
- Minimal static landing page at `wwwroot/index.html`
- Displays "SariKart API is running" status
- Serves static files including product and category images
- Images stored in:
  - `wwwroot/Images/Products/`
  - `wwwroot/Images/Categories/`

### API Response Format
Standard `Result` wrapper for responses:
```json
{
  "IsSuccess": true,
  "Message": "Operation completed successfully",
  "JsonResultObject": { ... }
}
```

## API Endpoints

### AppUsers
`GET/POST/PUT/DELETE api/AppUsers`

### Products
`GET/POST/PUT/DELETE api/Products`

### Categories
`GET/POST/PUT/DELETE api/Categories`

### ShopOrders
`GET/POST/PUT/DELETE api/ShopOrders`

### OrderLines
`GET/POST/PUT/DELETE api/OrderLines`

### Deliveries
`GET/POST/PUT/DELETE api/Deliveries`

### UserTypes
`GET/POST/PUT/DELETE api/UserTypes`

### OrderStatuses
`GET/POST/PUT/DELETE api/OrderStatuses`

### Vehicles
`GET/POST/PUT/DELETE api/Vehicles`

### StoreBranches
`GET/POST/PUT/DELETE api/StoreBranches`

### UserStores
`GET/POST/PUT/DELETE api/UserStores`

## Getting Started

### Prerequisites
- .NET 6.0 SDK
- SQL Server (or SQL Server Express)
- Visual Studio 2022 / VS Code

### Configuration

Update `appsettings.json` with your SQL Server connection string:
```json
{
  "ConnectionStrings": {
    "SariKartDb": "your-connection-string-here"
  }
}
```

### Setup Database

Run the schema and seed script located at `SariKart.sql` against your database.

### Run the Application

```bash
dotnet restore
dotnet run
```

The API will be available at:
- `http://localhost:5000`
- `http://192.168.1.2:5000`

### Access Swagger UI

When running in Development mode, visit:
```
http://localhost:5000/swagger
```

## Project Structure

```
SariKartAPIV2/
├── Controllers/          # API Controllers (REST endpoints)
├── Entities/             # Entity Framework models / DbContext
├── Models/               # DTOs and input/output models
├── wwwroot/              # Static files (images, index.html)
├── Images/               # Product and category images
├── Properties/           # Launch settings
├── SariKart.sql          # Database schema and seed data
├── Program.cs            # Application entry point
├── appsettings.json      # Application configuration
└── SariKartAPIV2.csproj  # Project file
```

## Default Credentials

| Role | Username | Password |
|------|----------|----------|
| Admin | admin | admin |
| Rider | rider | rider |
| Cashier | cashier | cashier |

## License

Proprietary
