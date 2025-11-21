using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using QuickCommerceAPI.Models;

public class Product
{
    [Key]
    public int ProductID { get; set; }

    public string ProductName { get; set; }

    public decimal ProductPrice { get; set; }

    public string ProductImg { get; set; }

    public bool IsInStock { get; set; }

    public int CategoryID { get; set; }

    public Category? Category { get; set; }

    [JsonIgnore]
    public ICollection<Cart>? Carts { get; set; }

    [JsonIgnore]
    public ICollection<OrderItem>? OrderItems { get; set; }

    [JsonIgnore]
    public ICollection<ProductReview>? ProductReviews { get; set; }
}
