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

// Load environment variables from .env file
DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

// ✅ Register DbContext with local SQL Server
builder.Services.AddDbContext<QuickCommerceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Add JWT authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
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

// ✅ Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Enable static file serving for wwwroot
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
    RequestPath = "/uploads"
});
app.UseCors();
app.UseAuthentication(); // Add this before UseAuthorization
app.UseAuthorization();
app.MapControllers();
app.Run();
