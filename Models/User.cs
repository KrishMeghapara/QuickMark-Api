using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QuickCommerceAPI.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int UserID { get; set; }

        public string UserName { get; set; } = string.Empty;

        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Profile Picture field
        public string? ProfilePicture { get; set; }

        // Google OAuth fields
        public string? GoogleId { get; set; }
        public string? GoogleName { get; set; }
        public string? GooglePicture { get; set; }
        public bool IsGoogleUser { get; set; } = false;

        // One-to-one relationship with Address
        public int? AddressID { get; set; }

        [JsonIgnore]
        public Address? Address { get; set; }

        [JsonIgnore]
        public ICollection<Order>? Orders { get; set; }

        [JsonIgnore]
        public ICollection<Cart>? Carts { get; set; }

        [JsonIgnore]
        public ICollection<ProductReview>? Reviews { get; set; }
    }
}
