using Microsoft.AspNetCore.Mvc;
using Quick_CommerceApiForEx.DTOs;
using QuickCommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Quick_CommerceApiForEx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly QuickCommerceDbContext _context;

        public TestController(QuickCommerceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "API is working!", timestamp = DateTime.UtcNow });
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }

        [HttpGet("db-test")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                var userCount = await _context.Users.CountAsync();
                return Ok(new { 
                    message = "Database connection successful", 
                    userCount = userCount,
                    timestamp = DateTime.UtcNow 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Database connection failed", 
                    error = ex.Message,
                    timestamp = DateTime.UtcNow 
                });
            }
        }

        [HttpPost("test-register")]
        public async Task<IActionResult> TestRegister([FromBody] RegisterDTO dto)
        {
            try
            {
                // Simple validation
                if (string.IsNullOrEmpty(dto.UserName) || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                {
                    return BadRequest(new { message = "All fields are required" });
                }

                // Check if user exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == dto.Email || u.UserName == dto.UserName);
                
                if (existingUser != null)
                {
                    return BadRequest(new { message = "User already exists" });
                }

                return Ok(new { 
                    message = "Test registration validation passed", 
                    data = new { dto.UserName, dto.Email },
                    timestamp = DateTime.UtcNow 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Test registration failed", 
                    error = ex.Message,
                    timestamp = DateTime.UtcNow 
                });
            }
        }

        [HttpGet("profile-picture-test")]
        public IActionResult TestProfilePicture()
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "profile-pictures");
            var files = Directory.GetFiles(uploadsPath, "*.jpg");
            
            return Ok(new
            {
                uploadsPath,
                files = files.Select(f => new
                {
                    fileName = Path.GetFileName(f),
                    fullPath = f,
                    url = $"/uploads/profile-pictures/{Path.GetFileName(f)}",
                    exists = System.IO.File.Exists(f)
                }).ToList()
            });
        }
    }
} 