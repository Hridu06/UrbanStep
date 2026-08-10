using System.ComponentModel.DataAnnotations;

namespace UrbanStep.DTOs.Product
{
    public class ProductInputDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Brand { get; set; } = string.Empty;

        // Top-level type: "shoes" | "clothing" | "accessories"
        [Required]
        public string Type { get; set; } = string.Empty;

        // Finer-grained category, e.g. "sneakers", "hoodies"
        [Required]
        public string Category { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Image { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public decimal? SalePrice { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public List<string> Colors { get; set; } = new();
        public List<string> Sizes { get; set; } = new();

        public bool IsNew { get; set; }
        public bool Featured { get; set; }
    }
}
