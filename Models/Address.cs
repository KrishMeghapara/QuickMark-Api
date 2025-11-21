using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // ✅ Required for JsonIgnore
using QuickCommerceAPI.Models;

[Table("Address")]
public class Address
{
    [Key]
    public int AddressID { get; set; }

    public string House { get; set; } // House/Flat/Building
    public string Street { get; set; } // Street/Area/Locality
    public string Landmark { get; set; } // Landmark (optional)
    public string City { get; set; }
    public string State { get; set; }
    public string Pincode { get; set; }
    public string Phone { get; set; } // Phone Number (optional)

    // One-to-one relationship with User
    public int? UserID { get; set; }

    [JsonIgnore]
    public User? User { get; set; }

    [JsonIgnore]
    public ICollection<Order>? Orders { get; set; }
}
