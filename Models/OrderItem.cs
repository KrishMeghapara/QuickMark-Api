using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using QuickCommerceAPI.Models;

[Table("OrderItem")]
public class OrderItem
{
    [Key]
    public int OrderItemID { get; set; }

    [ForeignKey("Order")]
    public int OrderID { get; set; }

    [ForeignKey("Product")]
    public int ProductID { get; set; }

    public int Quantity { get; set; }
    public decimal PriceAtTime { get; set; }

    // 🔒 Avoid circular reference issues
    [JsonIgnore]
    public Order? Order { get; set; }

    [JsonIgnore]
    public Product? Product { get; set; }
}
