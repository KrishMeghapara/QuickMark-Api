using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickCommerceAPI.Models;
using Quick_CommerceApiForEx.DTOs;

namespace Quick_CommerceApiForEx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly QuickCommerceDbContext _context;

        public ProductController(QuickCommerceDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return Ok(products);
        }

        // ✅ GET: api/Product/Filter
        [HttpGet("Filter")]
        public async Task<ActionResult<IEnumerable<Product>>> Filter([FromQuery] ProductFilterDTO filter)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryID == filter.CategoryId.Value);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.ProductPrice >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.ProductPrice <= filter.MaxPrice.Value);

            if (filter.InStockOnly == true)
                query = query.Where(p => p.IsInStock);

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(p => p.ProductName.Contains(filter.Search) || 
                                        p.Category.CategoryName.Contains(filter.Search));

            query = filter.SortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(p => p.ProductPrice),
                "price_desc" => query.OrderByDescending(p => p.ProductPrice),
                "name_asc" => query.OrderBy(p => p.ProductName),
                "name_desc" => query.OrderByDescending(p => p.ProductName),
                _ => query.OrderBy(p => p.ProductName)
            };

            var products = await query.Take(50).ToListAsync();
            return Ok(products);
        }

        // ✅ GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // ✅ POST: api/Product
        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromBody] CreateProductDTO dto)
        {
            try
            {
                var product = new Product
                {
                    ProductName = dto.ProductName,
                    ProductPrice = dto.ProductPrice,
                    ProductImg = dto.ProductImg ?? string.Empty,
                    CategoryID = dto.CategoryID,
                    IsInStock = dto.IsInStock
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Load the Category for the response
                await _context.Entry(product).Reference(p => p.Category).LoadAsync();

                return CreatedAtAction(nameof(GetById), new { id = product.ProductID }, product);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = "Could not create product: " + ex.InnerException?.Message });
            }
        }

        // ✅ PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDTO dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "Product not found." });

            // Update only provided fields
            if (!string.IsNullOrEmpty(dto.ProductName))
                product.ProductName = dto.ProductName;
            
            if (dto.ProductPrice.HasValue)
                product.ProductPrice = dto.ProductPrice.Value;
            
            if (!string.IsNullOrEmpty(dto.ProductImg))
                product.ProductImg = dto.ProductImg;
            
            if (dto.CategoryID.HasValue)
                product.CategoryID = dto.CategoryID.Value;
            
            if (dto.IsInStock.HasValue)
                product.IsInStock = dto.IsInStock.Value;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Product updated successfully." });
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Failed to update product. Please try again." });
            }
        }

        // ✅ DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "Product not found." });

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully." });
        }

        // ✅ GET: api/Product/ByCategory/2
        [HttpGet("ByCategory/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetByCategory(int categoryId)
        {
            // Check if category exists first
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryID == categoryId);
            
            if (!categoryExists)
                return NotFound(new { message = "Category not found." });

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryID == categoryId)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            return Ok(products);
        }

        // ✅ GET: api/Product/Search?query=apple
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<Product>>> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<Product>());

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductName.Contains(query) || 
                           p.Category.CategoryName.Contains(query))
                .Take(50)
                .ToListAsync();

            return Ok(products);
        }



        // ✅ GET: api/Product/PriceRange
        [HttpGet("PriceRange")]
        public async Task<ActionResult> GetPriceRange()
        {
            var minPrice = await _context.Products.MinAsync(p => p.ProductPrice);
            var maxPrice = await _context.Products.MaxAsync(p => p.ProductPrice);
            
            return Ok(new { minPrice, maxPrice });
        }
    }
}
