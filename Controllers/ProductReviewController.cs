using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickCommerceAPI.Models;

namespace Quick_CommerceApiForEx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly QuickCommerceDbContext _context;

        public ProductReviewController(QuickCommerceDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/ProductReview
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReview>>> GetAllReviews()
        {
            return await _context.ProductReviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // ✅ GET: api/ProductReview/Product/5
        [HttpGet("Product/{productId}")]
        public async Task<ActionResult<IEnumerable<ProductReview>>> GetByProduct(int productId)
        {
            var exists = await _context.Products.AnyAsync(p => p.ProductID == productId);
            if (!exists)
                return NotFound("Product not found.");

            var reviews = await _context.ProductReviews
                .Where(r => r.ProductID == productId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Ok(reviews);
        }

        // ✅ POST: api/ProductReview
        [HttpPost]
        public async Task<ActionResult> AddReview(ProductReview review)
        {
            // Optional: Prevent multiple reviews per user-product
            var alreadyReviewed = await _context.ProductReviews
                .AnyAsync(r => r.ProductID == review.ProductID && r.UserID == review.UserID);

            if (alreadyReviewed)
                return BadRequest("You have already reviewed this product.");

            review.CreatedAt = DateTime.Now;

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok("Review added.");
        }

        // ✅ PUT: api/ProductReview/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, ProductReview updatedReview)
        {
            if (id != updatedReview.ReviewID)
                return BadRequest("Review ID mismatch.");

            var review = await _context.ProductReviews.FindAsync(id);
            if (review == null)
                return NotFound();

            review.Rating = updatedReview.Rating;
            review.ReviewText = updatedReview.ReviewText;
            review.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok("Review updated.");
        }

        // ✅ DELETE: api/ProductReview/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.ProductReviews.FindAsync(id);
            if (review == null)
                return NotFound();

            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();

            return Ok("Review deleted.");
        }
    }
}
