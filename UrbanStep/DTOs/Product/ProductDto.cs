namespace UrbanStep.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;

        // Top-level type: "shoes" | "clothing" | "accessories"
        public string Type { get; set; } = string.Empty;

        // Finer-grained category, e.g. "sneakers", "hoodies"
        public string Category { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Image { get; set; }

        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public int Discount { get; set; }

        public double Rating { get; set; }
        public int Reviews { get; set; }

        public List<string> Colors { get; set; } = new();
        public List<string> Sizes { get; set; } = new();

        public int Stock { get; set; }

        public bool IsNew { get; set; }
        public bool IsOnSale { get; set; }
        public bool Featured { get; set; }
    }
}
