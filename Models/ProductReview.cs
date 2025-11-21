using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QuickCommerceAPI.Models
{
    [Table("ProductReview")]
    public class ProductReview
    {
        [Key]
        public int ReviewID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        public int Rating { get; set; }

        public string ReviewText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // 🔒 Prevent circular serialization
        [JsonIgnore]
        public Product? Product { get; set; }

        [JsonIgnore]
        public User? User { get; set; }
    }
}
