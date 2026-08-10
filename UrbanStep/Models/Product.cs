namespace UrbanStep.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        // Finer-grained category, e.g. "sneakers", "hoodies" (Category/CategoryId is the top-level type)
        public string Subcategory { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }

        public List<string> Colors { get; set; } = new();

        public List<string> Sizes { get; set; } = new();

        public double Rating { get; set; }

        public int ReviewsCount { get; set; }

        public bool IsNew { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int CategoryId { get; set; }

        // Navigation Property
        public Category Category { get; set; } = null!;
    }
}
