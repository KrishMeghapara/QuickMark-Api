using FluentValidation;
using Quick_CommerceApiForEx.DTOs;
using QuickCommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Quick_CommerceApiForEx.Validators
{
    public class LoginDTOValidator : AbstractValidator<LoginDTO>
    {
        public LoginDTOValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please enter a valid email address")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters");
        }
    }

    public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
    {
        private readonly QuickCommerceDbContext _context;

        public RegisterDTOValidator(QuickCommerceDbContext context)
        {
            _context = context;

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required")
                .Length(2, 50).WithMessage("Username must be between 2 and 50 characters")
                .Matches(@"^[a-zA-Z0-9_\s]+$").WithMessage("Username can only contain letters, numbers, spaces, and underscores");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please enter a valid email address")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Please confirm your password")
                .Equal(x => x.Password).WithMessage("Passwords do not match");

            // Check for existing username and email in the controller instead of validator
            // to avoid async validation issues
        }
    }

    public class UpdateUserDTOValidator : AbstractValidator<UpdateUserDTO>
    {
        private readonly QuickCommerceDbContext _context;

        public UpdateUserDTOValidator(QuickCommerceDbContext context)
        {
            _context = context;

            When(x => !string.IsNullOrEmpty(x.UserName), () =>
            {
                RuleFor(x => x.UserName)
                    .Length(2, 50).WithMessage("Username must be between 2 and 50 characters")
                    .Matches(@"^[a-zA-Z0-9_\s]+$").WithMessage("Username can only contain letters, numbers, spaces, and underscores");
            });

            When(x => !string.IsNullOrEmpty(x.Email), () =>
            {
                RuleFor(x => x.Email)
                    .EmailAddress().WithMessage("Please enter a valid email address")
                    .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");
            });
        }
    }

    public class ChangePasswordDTOValidator : AbstractValidator<ChangePasswordDTO>
    {
        public ChangePasswordDTOValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Please confirm your new password")
                .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
        }
    }
} 