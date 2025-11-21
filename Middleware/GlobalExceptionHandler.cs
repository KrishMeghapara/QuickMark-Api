using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;
using System.Net;

namespace Quick_CommerceApiForEx.Middleware
{
    public class GlobalExceptionHandler : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An unhandled exception occurred");

            var result = new ObjectResult(new
            {
                message = "An unexpected error occurred. Please try again later.",
                error = context.Exception.Message,
                timestamp = DateTime.UtcNow
            });

            switch (context.Exception)
            {
                case ValidationException validationException:
                    result.StatusCode = (int)HttpStatusCode.BadRequest;
                    result.Value = new
                    {
                        message = "Validation failed",
                        errors = validationException.Errors.Select(e => new
                        {
                            field = e.PropertyName,
                            message = e.ErrorMessage
                        }),
                        timestamp = DateTime.UtcNow
                    };
                    break;

                case ArgumentException:
                    result.StatusCode = (int)HttpStatusCode.BadRequest;
                    result.Value = new
                    {
                        message = context.Exception.Message,
                        timestamp = DateTime.UtcNow
                    };
                    break;

                case UnauthorizedAccessException:
                    result.StatusCode = (int)HttpStatusCode.Unauthorized;
                    result.Value = new
                    {
                        message = "Access denied",
                        timestamp = DateTime.UtcNow
                    };
                    break;

                default:
                    result.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
} 