namespace Quick_CommerceApiForEx.DTOs
{
    public class ProductFilterDTO
    {
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStockOnly { get; set; }
        public string? SortBy { get; set; } // "price_asc", "price_desc", "name_asc", "name_desc"
        public string? Search { get; set; }
    }
}