using Microsoft.EntityFrameworkCore;
using QuickCommerceAPI.Models;
using System.Text.Json.Serialization; // 🔁 Needed for ReferenceHandler
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation.AspNetCore;
using Quick_CommerceApiForEx.Validators;
using Quick_CommerceApiForEx.Middleware;
using Quick_CommerceApiForEx.Services;
using dotenv.net;

// Load environment variables from .env file (only in development)
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
{
    DotEnv.Load();
}

var builder = WebApplication.CreateBuilder(args);

// 🔧 Get connection string from environment variable or appsettings
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

// ✅ Register DbContext with PostgreSQL (for Render deployment)
builder.Services.AddDbContext<QuickCommerceDbContext>(options =>
    options.UseNpgsql(connectionString));

// ✅ Add controllers with cycle handling and FluentValidation
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionHandler>();
})
.AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles) // 🔁 FIX for circular reference
.AddFluentValidation(fv => 
{
    fv.RegisterValidatorsFromAssemblyContaining<LoginDTOValidator>();
    fv.DisableDataAnnotationsValidation = true; // Use FluentValidation instead of DataAnnotations
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Google OAuth service
builder.Services.AddHttpClient();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();

// Add Performance monitoring service
builder.Services.AddScoped<Quick_CommerceApiForEx.Services.PerformanceService>();

// 🔧 Get JWT settings from environment variables or appsettings
var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key") 
    ?? builder.Configuration["Jwt:Key"];
var jwtIssuer = Environment.GetEnvironmentVariable("Jwt__Issuer") 
    ?? builder.Configuration["Jwt:Issuer"];
var jwtAudience = Environment.GetEnvironmentVariable("Jwt__Audience") 
    ?? builder.Configuration["Jwt:Audience"];

// Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? ""))
    };
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ✅ Enable Swagger in all environments for API testing
app.UseSwagger();
app.UseSwaggerUI();

// 🏥 Health check endpoint for Render
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Only use HTTPS redirection in production with proper certificate
if (!app.Environment.IsDevelopment())
{
    // Note: Render handles HTTPS termination, so we don't need UseHttpsRedirection
}

app.UseStaticFiles(); // Enable static file serving for wwwroot

// Ensure uploads directory exists
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseCors();
app.UseAuthentication(); // Add this before UseAuthorization
app.UseAuthorization();
app.MapControllers();
app.Run();
