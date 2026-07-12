# SariKart-API

**SariKart-API** is an ASP.NET Core Web API backend for a **sari-sari store / mini-grocery e-commerce and delivery management system**. It powers a multi-role retail platform where customers can browse products by category, place orders with delivery, and where admins/cashiers manage products, categories, users, store branches, deliveries, and sales reporting.

## Tech Stack

| Technology | Detail |
|---|---|
| **Language** | C# (nullable enabled, implicit usings) |
| **Framework** | ASP.NET Core Web API **.NET 8.0** |
| **ORM** | Entity Framework Core **8.0.7** |
| **Database** | Microsoft SQL Server |
| **API Docs** | Swagger / OpenAPI (Swashbuckle.AspNetCore) |
| **JSON** | System.Text.Json |
| **File Storage** | Local filesystem (`Images/` directory) |

## Features

### Roles & Identity
- **Customer** — Browse products, place orders, manage profile
- **Admin** — Manage products, categories, users, branches, view reports
- **Rider** — View assigned deliveries
- **Cashier** — Process orders, manage payments

### Product Management
- Browse products by category and keyword
- Add / edit products with image upload (IFormFile)
- Image storage served via dedicated endpoints

### Order & Delivery Workflow
- Place order (auto-decrements stock)
- Default delivery record created automatically
- Admin assigns branch, rider, vehicle, delivery date
- Record payment amount and change
- Status transitions: Pending → Processing → Followed Up / Cancelled

### Reporting
- Monthly product sales history (overall + per branch)

### Other
- Category management with icon upload
- Store branch management
- Vehicle registry
- Image upload & retrieval for products/categories

## Quick Start

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Microsoft SQL Server
- Visual Studio 2022 / VS Code / Rider

### Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/SariKart-API.git
   cd SariKart-API
   ```

2. Configure the database connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "SariKartDb": "Server=YOUR_SERVER;Database=YOUR_DB;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

3. Run the SQL script to create the schema:
   ```bash
   # Execute SariKart.sql against your SQL Server instance
   ```

4. Restore and run:
   ```bash
   dotnet restore
   dotnet run
   ```

5. Open Swagger UI:
   ```
   https://localhost:5001/swagger
   ```

## API Endpoints

### Users (`api/user`)
| Method | Route | Purpose |
|---|---|---|
| POST | `/login` | Login with username & password |
| GET | `/{id}` | Get user by ID |
| POST | `/add` | Register new customer |
| PUT | `/edit` | Update customer profile |
| PUT | `/editAdminUser` | Update admin user |
| PUT | `/editContact` | Update contact info |
| PUT | `/changePassword` | Change password |
| GET | `/adminUser/{id}/{name}` | Search admin users |
| GET | `/list/riders` | List all riders |
| GET | `/rider/{name}` | Search riders |
| GET | `/cashier/{name}` | Search cashiers |
| POST | `/adminUser/add` | Add admin |
| POST | `/rider/add` | Add rider |
| POST | `/cashier/add` | Add cashier |
| PUT | `/editAdminUser` | Full admin update |
| GET | `/adminUser/get/{id}` | Get single admin |
| DELETE | `/adminUser/delete/{id}` | Delete admin |

### Products (`api/product`)
| Method | Route | Purpose |
|---|---|---|
| GET | `/{categoryId}/{productKeyword}` | Search products by category & keyword |
| GET | `/{id}` | Get single product |
| POST | `/add` | Add product with image upload |
| PUT | `/edit` | Update product (optional image replace) |
| GET | `/productHistory/{year}/{month}` | Monthly sales report |
| GET | `/productHistory/{branchId}/{year}/{month}` | Monthly sales report by branch |
| GET | `/getProductImage/{id}/{filename}` | Serve product image |

### Orders (`api/order`)
| Method | Route | Purpose |
|---|---|---|
| GET | `/{userId}/{orderStatusId}` | User's orders by status |
| POST | `/adminOrders` | Admin order search |
| GET | `/{orderId}` | Order detail |
| GET | `/orderStatus` | All order statuses |
| POST | `/add/{isCashOnHand}` | Place new order |
| POST | `/delivery/save` | Assign / update delivery |
| GET | `/delivery/{orderId}` | Get delivery for order |
| PUT | `/followUp/{orderId}` | Set status = Followed Up |
| PUT | `/cancel/{orderId}` | Cancel order |
| PUT | `/editOrderContact` | Update order contact info |

### Categories (`api/category`)
| Method | Route | Purpose |
|---|---|---|
| GET | `/` | All categories |
| GET | `/list` | Category list (Id + name) |
| POST | `/add` | Add category with icon |
| PUT | `/edit` | Update category + icon |
| GET | `/getCategoryImage/{id}/{filename}` |Serve category icon |

### Branches (`api/branch`)
| Method | Route | Purpose |
|---|---|---|
| GET | `/` | All branches |
| GET | `/list` | Branch list (Id + name) |
| GET | `/{id}` | Single branch |
| POST | `/add` | Add branch |
| PUT | `/edit` | Update branch |
| DELETE | `/delete/{id}` | Delete branch |

### Deliveries (`api/delivery`)
| Method | Route | Purpose |
|---|---|---|
| GET | `/{orderId}` | Get delivery by order ID |

### Vehicles (`api/vehicle`)
| Method | Route | Purpose |
|---|---|---|
| GET | `/list` | List all vehicles |

## Response Format

All endpoints return a standard envelope:
```json
{
  "JsonResultObject": {},
  "Message": "Success",
  "IsSuccess": true
}
```

## Reference Values

### User Types
| Id | Role |
|---|---|
| 1 | Customer |
| 2 | Admin |
| 3 | Rider |
| 1003 | Cashier |

### Order Statuses
| Id | Status |
|---|---|
| 1 | Pending |
| 2 | Processing |
| 3 | Cancelled |
| 4 | Followed Up |

### Demo Credentials
| Username | Password | Role |
|---|---|---|
| admin | admin | Admin |
| rider | rider | Rider |
| cashier | cashier | Cashier |

## Image Storage

Product and category images are stored on the server filesystem:
- `Images/Products/{productId}/{filename}.png`
- `Images/Categories/{categoryId}/{filename}.png`

**Important:** Images uploaded at runtime are **not** tracked in Git. Ensure the `Images/` directory structure exists and is writable by the application.

## Security Notes

> ⚠️ This project is intended for **local / development use** only and contains several security issues:

- **Plaintext credentials:** Database connection string is stored in plaintext in `appsettings.json`.
- **Plaintext passwords:** User passwords are stored in plaintext (`NVARCHAR(50)`).
- **No authentication:** No JWT, cookies, or Identity framework. Login is a plaintext DB lookup.
- **Wide-open CORS:** `AllowAnyOrigin` is configured globally.
- **No authorization enforcement:** `UseAuthorization()` is registered but no `[Authorize]` attributes or authentication scheme is configured. Role enforcement is implicit (which endpoint is called).

**Before deploying to production, implement proper authentication, hashed passwords, restricted CORS, and connection string management.**

## License

[Specify your license here]
