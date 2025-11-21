using FluentValidation;
using Quick_CommerceApiForEx.DTOs;
using QuickCommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Quick_CommerceApiForEx.Validators
{
    public class CreateProductDTOValidator : AbstractValidator<CreateProductDTO>
    {
        private readonly QuickCommerceDbContext _context;

        public CreateProductDTOValidator(QuickCommerceDbContext context)
        {
            _context = context;

            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required")
                .Length(2, 200).WithMessage("Product name must be between 2 and 200 characters")
                .MustAsync(async (productName, cancellation) =>
                {
                    if (string.IsNullOrEmpty(productName)) return true;
                    return !await _context.Products.AnyAsync(p => p.ProductName == productName, cancellation);
                }).WithMessage("Product name already exists");

            RuleFor(x => x.ProductPrice)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .LessThanOrEqualTo(999999.99m).WithMessage("Price cannot exceed 999,999.99");

            RuleFor(x => x.CategoryID)
                .GreaterThan(0).WithMessage("Category ID must be greater than 0")
                .MustAsync(async (categoryId, cancellation) =>
                {
                    return await _context.Categories.AnyAsync(c => c.CategoryID == categoryId, cancellation);
                }).WithMessage("Category does not exist");

            When(x => !string.IsNullOrEmpty(x.ProductImg), () =>
            {
                RuleFor(x => x.ProductImg)
                    .MaximumLength(500).WithMessage("Product image URL cannot exceed 500 characters")
                    .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                    .WithMessage("Please enter a valid image URL");
            });
        }
    }

    public class UpdateProductDTOValidator : AbstractValidator<UpdateProductDTO>
    {
        private readonly QuickCommerceDbContext _context;

        public UpdateProductDTOValidator(QuickCommerceDbContext context)
        {
            _context = context;

            When(x => !string.IsNullOrEmpty(x.ProductName), () =>
            {
                RuleFor(x => x.ProductName)
                    .Length(2, 200).WithMessage("Product name must be between 2 and 200 characters");
            });

            When(x => x.ProductPrice.HasValue, () =>
            {
                RuleFor(x => x.ProductPrice!.Value)
                    .GreaterThan(0).WithMessage("Price must be greater than 0")
                    .LessThanOrEqualTo(999999.99m).WithMessage("Price cannot exceed 999,999.99");
            });

            When(x => x.CategoryID.HasValue, () =>
            {
                RuleFor(x => x.CategoryID!.Value)
                    .GreaterThan(0).WithMessage("Category ID must be greater than 0")
                    .MustAsync(async (categoryId, cancellation) =>
                    {
                        return await _context.Categories.AnyAsync(c => c.CategoryID == categoryId, cancellation);
                    }).WithMessage("Category does not exist");
            });

            When(x => !string.IsNullOrEmpty(x.ProductImg), () =>
            {
                RuleFor(x => x.ProductImg)
                    .MaximumLength(500).WithMessage("Product image URL cannot exceed 500 characters")
                    .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                    .WithMessage("Please enter a valid image URL");
            });
        }
    }
} 