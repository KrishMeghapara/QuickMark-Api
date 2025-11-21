using System.ComponentModel.DataAnnotations;

namespace Quick_CommerceApiForEx.DTOs
{
    public class AddToCartDTO
    {
        [Required(ErrorMessage = "Product ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }
    }

    public class UpdateCartQuantityDTO
    {
        [Required(ErrorMessage = "Cart ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Cart ID must be greater than 0")]
        public int CartID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }
    }
} 