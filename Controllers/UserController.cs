using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickCommerceAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Quick_CommerceApiForEx.DTOs;
using Quick_CommerceApiForEx.Services;

namespace Quick_CommerceApiForEx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly QuickCommerceDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IGoogleAuthService _googleAuthService;

        public UserController(QuickCommerceDbContext context, IConfiguration configuration, IWebHostEnvironment env, IGoogleAuthService googleAuthService)
        {
            _context = context;
            _configuration = configuration;
            _env = env;
            _googleAuthService = googleAuthService;
        }

        // ✅ POST: api/User/Register
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            try
            {
                // Manual validation to avoid FluentValidation async issues
                if (string.IsNullOrEmpty(dto.UserName) || dto.UserName.Length < 2 || dto.UserName.Length > 50)
                {
                    return BadRequest(new { message = "Username must be between 2 and 50 characters" });
                }

                if (string.IsNullOrEmpty(dto.Email) || !dto.Email.Contains("@"))
                {
                    return BadRequest(new { message = "Please enter a valid email address" });
                }

                if (string.IsNullOrEmpty(dto.Password) || dto.Password.Length < 6)
                {
                    return BadRequest(new { message = "Password must be at least 6 characters" });
                }

                if (dto.Password != dto.ConfirmPassword)
                {
                    return BadRequest(new { message = "Passwords do not match" });
                }

                // Check if username already exists
                var existingUserByUsername = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == dto.UserName);
                if (existingUserByUsername != null)
                {
                    return BadRequest(new { message = "Username is already taken" });
                }

                // Check if email already exists
                var existingUserByEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (existingUserByEmail != null)
                {
                    return BadRequest(new { message = "Email is already registered" });
                }

                var user = new User
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    PasswordHash = PasswordHelper.HashPassword(dto.Password)
                    // AddressID is not set during registration - user will add address later
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true,
                    message = "User registered successfully.",
                    user = new { 
                        UserID = user.UserID, 
                        UserName = user.UserName, 
                        Email = user.Email 
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false,
                    message = "Registration failed. Please try again later.",
                    error = ex.Message 
                });
            }
        }

        // ✅ POST: api/User/Login
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return Ok(new { Token = jwtToken, user.UserID, user.UserName, user.Email });
        }

        // ✅ POST: api/User/GoogleLogin
        [HttpPost("GoogleLogin")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO dto)
        {
            try
            {
                // Verify Google token
                var googleUserInfo = await _googleAuthService.VerifyGoogleTokenAsync(dto.IdToken);
                
                if (googleUserInfo == null)
                {
                    return Unauthorized("Invalid Google token.");
                }

                // Check if user exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == googleUserInfo.Email);

                User user;

                if (existingUser != null)
                {
                    // User exists, update Google info if needed
                    if (!existingUser.IsGoogleUser)
                    {
                        existingUser.IsGoogleUser = true;
                        existingUser.GoogleId = googleUserInfo.Sub;
                        existingUser.GoogleName = googleUserInfo.Name;
                        existingUser.GooglePicture = googleUserInfo.Picture;
                        await _context.SaveChangesAsync();
                    }
                    user = existingUser;
                }
                else
                {
                    // Create new user
                    user = new User
                    {
                        UserName = googleUserInfo.Name,
                        Email = googleUserInfo.Email,
                        GoogleId = googleUserInfo.Sub,
                        GoogleName = googleUserInfo.Name,
                        GooglePicture = googleUserInfo.Picture,
                        IsGoogleUser = true,
                        PasswordHash = string.Empty // Google users don't need password
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                // Generate JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email)
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                return Ok(new { 
                    Token = jwtToken, 
                    user.UserID, 
                    user.UserName, 
                    user.Email,
                    user.GooglePicture,
                    IsGoogleUser = user.IsGoogleUser
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Google login failed: {ex.Message}");
            }
        }

        // ✅ GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return await _context.Users.Include(u => u.Address).ToListAsync();
        }

        // ✅ GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null)
                return NotFound();

            return user;
        }

        // ✅ PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            // Update only provided fields
            if (!string.IsNullOrEmpty(dto.UserName))
                user.UserName = dto.UserName;
            
            if (!string.IsNullOrEmpty(dto.Email))
                user.Email = dto.Email;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "User updated successfully." });
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Failed to update user. Please try again." });
            }
        }

        // ✅ DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully." });
        }

        // ✅ GET: api/User/Profile
        [HttpGet("Profile")]
        [Authorize]
        public async Task<ActionResult<User>> GetCurrentUserProfile()
        {
            try
            {
                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                var user = await _context.Users
                    .Include(u => u.Address)
                    .FirstOrDefaultAsync(u => u.UserID == userId);

                if (user == null)
                    return NotFound(new { message = "User not found." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving profile: {ex.Message}" });
            }
        }

        // ✅ POST: api/User/ChangePassword
        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            try
            {
                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                // Verify current password
                if (!PasswordHelper.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                {
                    return BadRequest(new { message = "Current password is incorrect." });
                }

                // Update password
                user.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error changing password: {ex.Message}" });
            }
        }

        // ✅ GET: api/User/ValidateToken
        [HttpGet("ValidateToken")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            // If the request reaches here, the token is valid
            return Ok(new { valid = true });
        }


        // ✅ POST: api/User/UploadProfilePicture
        [HttpPost("UploadProfilePicture")]
        [Authorize]
        public async Task<ActionResult<ProfilePictureResponseDTO>> UploadProfilePicture(IFormFile file)
        {
            try
            {
                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                // Validate file
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new ProfilePictureResponseDTO 
                    { 
                        Success = false, 
                        Message = "No file was uploaded." 
                    });
                }

                // Check file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new ProfilePictureResponseDTO 
                    { 
                        Success = false, 
                        Message = "File size must be less than 5MB." 
                    });
                }

                // Check file type (only JPG/JPEG)
                var allowedExtensions = new[] { ".jpg", ".jpeg" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new ProfilePictureResponseDTO 
                    { 
                        Success = false, 
                        Message = "Only JPG/JPEG files are allowed." 
                    });
                }

                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "uploads", "profile-pictures");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Generate unique filename
                var fileName = $"profile_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Delete old profile picture if exists
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, user.ProfilePicture.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save new file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Update user's profile picture path in database
                var relativePath = $"/uploads/profile-pictures/{fileName}";
                user.ProfilePicture = relativePath;
                await _context.SaveChangesAsync();

                return Ok(new ProfilePictureResponseDTO
                {
                    Success = true,
                    Message = "Profile picture uploaded successfully.",
                    ProfilePictureUrl = relativePath,
                    ProfilePicturePath = relativePath
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProfilePictureResponseDTO
                {
                    Success = false,
                    Message = $"Error uploading profile picture: {ex.Message}"
                });
            }
        }

        // ✅ DELETE: api/User/RemoveProfilePicture
        [HttpDelete("RemoveProfilePicture")]
        [Authorize]
        public async Task<ActionResult<ProfilePictureResponseDTO>> RemoveProfilePicture()
        {
            try
            {
                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                // Delete file if exists
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    var filePath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, user.ProfilePicture.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Clear profile picture path in database
                user.ProfilePicture = null;
                await _context.SaveChangesAsync();

                return Ok(new ProfilePictureResponseDTO
                {
                    Success = true,
                    Message = "Profile picture removed successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProfilePictureResponseDTO
                {
                    Success = false,
                    Message = $"Error removing profile picture: {ex.Message}"
                });
            }
        }
    }
}
