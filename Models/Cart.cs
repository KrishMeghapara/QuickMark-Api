using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // ✅ Required for JsonIgnore
using QuickCommerceAPI.Models;

[Table("Cart")]
public class Cart
{
    [Key]
    public int CartID { get; set; }

    public int UserID { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    public int ProductID { get; set; }

    public Product? Product { get; set; }

    public int Quantity { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow; // 🕒 Default value
}
