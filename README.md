# QuickMart API 🚀

Backend API for QuickMart - A Quick Commerce Platform delivering groceries in 12 minutes.

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core Web API
- **Database:** SQL Server
- **Authentication:** JWT + Google OAuth 2.0
- **ORM:** Entity Framework Core
- **Architecture:** RESTful API

## 📋 Features

### Authentication & Authorization
- ✅ JWT Token-based authentication
- ✅ Google OAuth 2.0 integration
- ✅ User registration and login
- ✅ Password change functionality
- ✅ Secure token management

### User Management
- ✅ User profile management
- ✅ Profile picture upload
- ✅ Address management (CRUD operations)
- ✅ Multiple address support
- ✅ Default address selection

### Product Management
- ✅ Product CRUD operations
- ✅ Category-based product filtering
- ✅ Product search functionality
- ✅ Product image management
- ✅ Stock management

### Order Management
- ✅ Order creation and tracking
- ✅ Order history
- ✅ Order status updates
- ✅ Order items management

### Cart Management
- ✅ Add/Remove items from cart
- ✅ Update cart quantities
- ✅ Cart persistence
- ✅ Cart total calculation

## 🚀 Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- SQL Server (LocalDB or full version)
- Visual Studio 2022 or VS Code
- Git

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/KrishMeghapara/QuickMart-API.git
   cd QuickMart-API
   ```

2. **Update Connection String**
   
   Edit `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=QuickCommerceDB;Trusted_Connection=true;TrustServerCertificate=true"
     }
   }
   ```

3. **Update Database**
   ```bash
   dotnet ef database update
   ```

4. **Configure Google OAuth (Optional)**
   
   Add your Google OAuth credentials in `appsettings.json`:
   ```json
   {
     "GoogleAuth": {
       "ClientId": "your-client-id",
       "ClientSecret": "your-client-secret"
     }
   }
   ```

5. **Run the API**
   ```bash
   dotnet run
   ```

   The API will be available at: `http://localhost:5236`

## 📚 API Documentation

### Base URL
```
http://localhost:5236/api
```

### Authentication Endpoints

#### Register User
```http
POST /api/User/Register
Content-Type: application/json

{
  "userName": "string",
  "email": "string",
  "password": "string",
  "confirmPassword": "string"
}
```

#### Login
```http
POST /api/User/Login
Content-Type: application/json

{
  "email": "string",
  "password": "string"
}
```

#### Google Login
```http
POST /api/User/GoogleLogin
Content-Type: application/json

{
  "idToken": "string"
}
```

### User Endpoints

#### Get User Profile
```http
GET /api/User/Profile
Authorization: Bearer {token}
```

#### Update Profile Picture
```http
POST /api/User/UploadProfilePicture
Authorization: Bearer {token}
Content-Type: multipart/form-data

file: [image file]
```

#### Change Password
```http
POST /api/User/ChangePassword
Authorization: Bearer {token}
Content-Type: application/json

{
  "currentPassword": "string",
  "newPassword": "string"
}
```

### Address Endpoints

#### Get User Address
```http
GET /api/Address/GetForCurrentUser
Authorization: Bearer {token}
```

#### Add Address
```http
POST /api/Address/Add
Authorization: Bearer {token}
Content-Type: application/json

{
  "house": "string",
  "street": "string",
  "landmark": "string",
  "city": "string",
  "state": "string",
  "pincode": "string",
  "phone": "string"
}
```

#### Update Address
```http
PUT /api/Address/UpdateForCurrentUser
Authorization: Bearer {token}
Content-Type: application/json

{
  "house": "string",
  "street": "string",
  "landmark": "string",
  "city": "string",
  "state": "string",
  "pincode": "string",
  "phone": "string"
}
```

#### Delete Address
```http
DELETE /api/Address/Delete/{id}
Authorization: Bearer {token}
```

### Product Endpoints

#### Get All Products
```http
GET /api/Product/GetAll
```

#### Get Product by ID
```http
GET /api/Product/GetById/{id}
```

#### Get Products by Category
```http
GET /api/Product/GetByCategory/{categoryId}
```

#### Search Products
```http
GET /api/Product/Search?query={searchTerm}
```

#### Filter Products
```http
POST /api/Product/Filter
Content-Type: application/json

{
  "categoryId": 0,
  "minPrice": 0,
  "maxPrice": 0,
  "inStockOnly": true,
  "sortBy": "string"
}
```

### Category Endpoints

#### Get All Categories
```http
GET /api/Category/GetAll
```

#### Get Category by ID
```http
GET /api/Category/GetById/{id}
```

### Order Endpoints

#### Create Order
```http
POST /api/Order/Create
Authorization: Bearer {token}
Content-Type: application/json

{
  "addressID": 0,
  "items": [
    {
      "productID": 0,
      "quantity": 0
    }
  ]
}
```

#### Get User Orders
```http
GET /api/Order/User/{userId}
Authorization: Bearer {token}
```

#### Get Order by ID
```http
GET /api/Order/GetById/{id}
Authorization: Bearer {token}
```

## 🗄️ Database Schema

### Tables
- **Users** - User accounts and authentication
- **Addresses** - User delivery addresses
- **Categories** - Product categories
- **Products** - Product catalog
- **Orders** - Order information
- **OrderItems** - Order line items
- **Cart** - Shopping cart items

## 🔒 Security

- JWT token-based authentication
- Password hashing with BCrypt
- CORS configuration for frontend
- Secure file upload validation
- SQL injection prevention via EF Core
- XSS protection

## 🌐 CORS Configuration

The API is configured to accept requests from:
- `http://localhost:5173` (Vite dev server)
- `http://localhost:3000` (React dev server)

Update `Program.cs` to add more origins if needed.

## 📦 Project Structure

```
Quick-CommerceApiForEx/
├── Controllers/          # API Controllers
├── Models/              # Data models
├── Data/                # Database context
├── Services/            # Business logic
├── Migrations/          # EF Core migrations
├── wwwroot/            # Static files (uploads)
├── appsettings.json    # Configuration
└── Program.cs          # Application entry point
```

## 🚀 Deployment

### Deploy to Azure

1. Create an Azure App Service
2. Configure connection string in Azure
3. Deploy using Visual Studio or Azure CLI
4. Update CORS settings for production URL

### Deploy to IIS

1. Publish the application
2. Configure IIS with .NET Core hosting bundle
3. Set up application pool
4. Configure connection string

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License.

## 👨‍💻 Author

**Krish Meghapara**
- GitHub: [@KrishMeghapara](https://github.com/KrishMeghapara)

## 🔗 Related Repositories

- [QuickMart Frontend](https://github.com/KrishMeghapara/QuickMart) - React frontend application

## 📞 Support

For support, email your-email@example.com or create an issue in the repository.

---

Made with ❤️ for Quick Commerce
