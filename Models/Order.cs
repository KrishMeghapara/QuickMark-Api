using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QuickCommerceAPI.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        [ForeignKey("Address")]
        public int AddressID { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime OrderDate { get; set; }

        // Navigation properties
        [JsonIgnore] // 🔄 Add this if User causes cycles or validation issues
        public User? User { get; set; }

        [JsonIgnore] // 🔄 Add this if Address causes cycles or validation issues
        public Address? Address { get; set; }

        // ✅ Always safe to initialize collection
        [JsonIgnore] // 🔄 Optional: prevent sending deep nested data unless needed
        public ICollection<OrderItem>? Items { get; set; } = new List<OrderItem>();
    }
}
