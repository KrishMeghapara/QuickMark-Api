using FluentValidation;
using Quick_CommerceApiForEx.DTOs;
using QuickCommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Quick_CommerceApiForEx.Validators
{
    public class AddToCartDTOValidator : AbstractValidator<AddToCartDTO>
    {
        private readonly QuickCommerceDbContext _context;

        public AddToCartDTOValidator(QuickCommerceDbContext context)
        {
            _context = context;

            RuleFor(x => x.ProductID)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0")
                .Must(productId =>
                {
                    return _context.Products.Any(p => p.ProductID == productId);
                }).WithMessage("Product does not exist");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100");

            RuleFor(x => x)
                .Must(cartItem =>
                {
                    var product = _context.Products.Find(cartItem.ProductID);
                    return product?.IsInStock == true;
                }).WithMessage("Product is out of stock");
        }
    }

    public class UpdateCartQuantityDTOValidator : AbstractValidator<UpdateCartQuantityDTO>
    {
        private readonly QuickCommerceDbContext _context;

        public UpdateCartQuantityDTOValidator(QuickCommerceDbContext context)
        {
            _context = context;

            RuleFor(x => x.CartID)
                .GreaterThan(0).WithMessage("Cart ID must be greater than 0")
                .Must(cartId =>
                {
                    return _context.Carts.Any(c => c.CartID == cartId);
                }).WithMessage("Cart item does not exist");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100");
        }
    }
} 