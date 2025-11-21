# API Validation Documentation

This document describes the validation system implemented using FluentValidation for the Quick Commerce API.

## Overview

The API uses FluentValidation for comprehensive input validation, providing:
- Clear, user-friendly error messages
- Database-aware validation (e.g., checking if email already exists)
- Consistent error response format
- Global exception handling

## Validation Rules

### User Operations

#### Login
- **Email**: Required, valid email format, max 100 characters
- **Password**: Required, min 6 characters, max 100 characters

#### Registration
- **Username**: Required, 2-50 characters, alphanumeric + spaces + underscores, unique
- **Email**: Required, valid email format, max 100 characters, unique
- **Password**: Required, min 6 characters, max 100 characters, must contain uppercase, lowercase, and number
- **ConfirmPassword**: Required, must match Password

#### Update User
- **Username**: Optional, 2-50 characters, alphanumeric + spaces + underscores
- **Email**: Optional, valid email format, max 100 characters

### Product Operations

#### Create Product
- **ProductName**: Required, 2-200 characters, unique
- **ProductPrice**: Required, greater than 0, max 999,999.99
- **CategoryID**: Required, must exist in database
- **ProductImg**: Optional, max 500 characters, valid URL format
- **IsInStock**: Optional, defaults to true

#### Update Product
- **ProductName**: Optional, 2-200 characters
- **ProductPrice**: Optional, greater than 0, max 999,999.99
- **CategoryID**: Optional, must exist in database
- **ProductImg**: Optional, max 500 characters, valid URL format
- **IsInStock**: Optional

### Cart Operations

#### Add to Cart
- **UserID**: Required, must exist in database
- **ProductID**: Required, must exist in database
- **Quantity**: Required, 1-100, product must be in stock

#### Update Cart Quantity
- **CartID**: Required, must exist in database
- **Quantity**: Required, 1-100

## Error Response Format

### Validation Errors
```json
{
  "message": "Validation failed",
  "errors": [
    {
      "field": "Email",
      "message": "Please enter a valid email address"
    },
    {
      "field": "Password",
      "message": "Password must be at least 6 characters"
    }
  ],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### General Errors
```json
{
  "message": "User not found",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## Frontend Integration

The frontend API service automatically handles validation errors and displays them to users in a user-friendly format.

### Example Usage

```javascript
try {
  const result = await apiService.register({
    UserName: "john_doe",
    Email: "john@example.com",
    Password: "Password123",
    ConfirmPassword: "Password123"
  });
  console.log("Registration successful:", result);
} catch (error) {
  // Error message will be formatted for display
  console.error("Registration failed:", error.message);
}
```

## Benefits

1. **Consistent Validation**: All endpoints use the same validation framework
2. **Database Integration**: Validators can check against database state
3. **Clear Error Messages**: User-friendly error messages
4. **Type Safety**: Strongly typed DTOs
5. **Maintainable**: Centralized validation rules
6. **Extensible**: Easy to add new validation rules

## Adding New Validators

1. Create DTO in `DTOs/` folder
2. Create validator in `Validators/` folder
3. Register validator in `Program.cs` (automatic with current setup)
4. Update controller to use DTO

Example:
```csharp
public class MyDTOValidator : AbstractValidator<MyDTO>
{
    public MyDTOValidator()
    {
        RuleFor(x => x.Property)
            .NotEmpty().WithMessage("Property is required")
            .Length(2, 50).WithMessage("Property must be between 2 and 50 characters");
    }
}
``` 