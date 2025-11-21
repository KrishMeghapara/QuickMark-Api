using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickCommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Quick_CommerceApiForEx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly QuickCommerceDbContext _context;

        public AddressController(QuickCommerceDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/Address
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAll()
        {
            return await _context.Addresses.ToListAsync();
        }

        // ✅ GET: api/Address/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetById(int id)
        {
            var address = await _context.Addresses.FindAsync(id);

            if (address == null)
                return NotFound();

            return address;
        }

        // ✅ GET: api/Address/User/3
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Address>>> GetByUserId(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user?.Address == null)
                return NotFound("Address not found for this user.");

            return Ok(user.Address);
        }

        // ✅ POST: api/Address
        [HttpPost]
        public async Task<ActionResult<Address>> Create(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = address.AddressID }, address);
        }

        // POST: api/Address/AddForCurrentUser
        [HttpPost("AddForCurrentUser")]
        [Authorize]
        public async Task<IActionResult> AddForCurrentUser(Address address)
        {
            // Get user ID from JWT
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Invalid user ID in token.");

            // Check if user already has an address
            var existingAddress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserID == userId);

            if (existingAddress != null)
                return BadRequest("User already has an address. Use update instead.");

            // Set the UserID for the address
            address.UserID = userId;

            // Create address
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            // Update user's AddressID reference
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.AddressID = address.AddressID;
                await _context.SaveChangesAsync();
            }

            return Ok(address);
        }

        // PUT: api/Address/UpdateForCurrentUser
        [HttpPut("UpdateForCurrentUser")]
        [Authorize]
        public async Task<IActionResult> UpdateForCurrentUser(Address address)
        {
            // Get user ID from JWT
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Invalid user ID in token.");

            // Find user's existing address
            var existingAddress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserID == userId);

            if (existingAddress == null)
                return NotFound("No address found for this user. Add an address first.");

            // Update the existing address
            existingAddress.City = address.City;
            existingAddress.State = address.State;
            existingAddress.Pincode = address.Pincode;

            await _context.SaveChangesAsync();

            return Ok(existingAddress);
        }

        // GET: api/Address/GetForCurrentUser
        [HttpGet("GetForCurrentUser")]
        [Authorize]
        public async Task<IActionResult> GetForCurrentUser()
        {
            // Get user ID from JWT
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Invalid user ID in token.");

            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserID == userId);

            if (address == null)
                return NotFound("No address found for this user.");

            return Ok(address);
        }

        // ✅ PUT: api/Address/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Address address)
        {
            if (id != address.AddressID)
                return BadRequest("ID mismatch.");

            _context.Entry(address).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Addresses.Any(a => a.AddressID == id))
                    return NotFound();

                throw;
            }

            return Ok("Address updated.");
        }

        // ✅ DELETE: api/Address/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
                return NotFound();

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return Ok("Address deleted.");
        }
    }
}
