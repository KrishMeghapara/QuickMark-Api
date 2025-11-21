using System.ComponentModel.DataAnnotations;

namespace Quick_CommerceApiForEx.DTOs
{
    public class CreateProductDTO
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 200 characters")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal ProductPrice { get; set; }

        [StringLength(500, ErrorMessage = "Product image URL cannot exceed 500 characters")]
        public string? ProductImg { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
        public int CategoryID { get; set; }

        public bool IsInStock { get; set; } = true;
    }

    public class UpdateProductDTO
    {
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 200 characters")]
        public string? ProductName { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? ProductPrice { get; set; }

        [StringLength(500, ErrorMessage = "Product image URL cannot exceed 500 characters")]
        public string? ProductImg { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
        public int? CategoryID { get; set; }

        public bool? IsInStock { get; set; }
    }
} 