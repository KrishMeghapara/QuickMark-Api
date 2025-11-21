using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickCommerceAPI.Models;
using Quick_CommerceApiForEx.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Quick_CommerceApiForEx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly QuickCommerceDbContext _context;

        public CartController(QuickCommerceDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/Cart/User/3
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCartByUserId(int userId)
        {
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                    .ThenInclude(p => p.Category)
                .Where(c => c.UserID == userId)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
                return NotFound("No items in cart for this user.");

            return Ok(cartItems);
        }

        // ✅ GET: api/Cart/MyCart
        [HttpGet("MyCart")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetMyCart()
        {
            try
            {
                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                var cartItems = await _context.Carts
                    .Include(c => c.Product)
                        .ThenInclude(p => p.Category)
                    .Where(c => c.UserID == userId)
                    .ToListAsync();

                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving cart: {ex.Message}" });
            }
        }

        // ✅ POST: api/Cart/Add
        [HttpPost("Add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO dto)
        {
            try
            {
                // Get user ID from JWT token
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid token." });
                }

                var existingItem = await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == dto.ProductID);

                if (existingItem != null)
                {
                    existingItem.Quantity += dto.Quantity;
                    existingItem.AddedAt = DateTime.Now;
                    _context.Update(existingItem);
                }
                else
                {
                    var cartItem = new Cart
                    {
                        UserID = userId,
                        ProductID = dto.ProductID,
                        Quantity = dto.Quantity,
                        AddedAt = DateTime.Now
                    };
                    _context.Carts.Add(cartItem);
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Product added to cart successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error adding to cart: {ex.Message}" });
            }
        }

        // ✅ PUT: api/Cart/UpdateQty
        [HttpPut("UpdateQty")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartQuantityDTO dto)
        {
            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.CartID == dto.CartID);

            if (cartItem == null)
                return NotFound(new { message = "Cart item not found." });

            cartItem.Quantity = dto.Quantity;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Quantity updated successfully." });
        }

        // ✅ DELETE: api/Cart/Remove/5
        [HttpDelete("Remove/{cartId}")]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem == null)
                return NotFound(new { message = "Cart item not found." });

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Item removed from cart successfully." });
        }
    }
}
