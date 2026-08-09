namespace UrbanStep.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;

        public Product Product { get; set; } = null!;
    }
}