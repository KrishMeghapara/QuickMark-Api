using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

[Table("Category")]
public class Category
{
    [Key]
    public int CategoryID { get; set; }

    [Required]
    public string CategoryName { get; set; }

    [JsonIgnore]
    public ICollection<Product>? Products { get; set; }
}
