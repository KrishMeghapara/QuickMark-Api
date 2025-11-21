using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickCommerceAPI.Models;

namespace Quick_CommerceApiForEx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly QuickCommerceDbContext _context;

        public OrderController(QuickCommerceDbContext context)
        {
            _context = context;
        }

        // ✅ POST: api/Order/BuyNow
        [HttpPost("BuyNow")]
        public async Task<IActionResult> BuyNow([FromBody] BuyNowDTO dto)
        {
            if (dto == null || dto.Items == null || dto.Items.Count == 0)
                return BadRequest("Invalid order data.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Step 1: Create order
                var order = new Order
                {
                    UserID = dto.UserID,
                    AddressID = dto.AddressID,
                    TotalAmount = dto.TotalAmount,
                    OrderDate = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // To get the generated OrderID

                // Step 2: Add order items
                foreach (var item in dto.Items)
                {
                    var orderItem = new OrderItem
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        PriceAtTime = item.PriceAtTime
                    };
                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { Message = "Order placed successfully", OrderID = order.OrderID });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Order failed: {ex.Message}");
            }
        }

        // ✅ GET: api/Order/User/3
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUser(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            if (orders == null || orders.Count == 0)
                return NotFound("No orders found for the user.");

            return Ok(orders);
        }
    }

    // ✅ DTOs
    public class BuyNowDTO
    {
        public int UserID { get; set; }
        public int AddressID { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDTO> Items { get; set; } = new();
    }

    public class OrderItemDTO
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtTime { get; set; }
    }
}
