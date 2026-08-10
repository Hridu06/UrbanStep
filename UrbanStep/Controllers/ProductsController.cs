using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrbanStep.Data;
using UrbanStep.DTOs.Product;
using UrbanStep.Models;

namespace UrbanStep.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProductsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetAll()
        {
            var products = await _db.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return products.Select(ToDto).ToList();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null || !product.IsActive)
                return NotFound();

            return ToDto(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create(ProductInputDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var category = await GetOrCreateCategory(dto.Type);

            var product = new Product
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Subcategory = dto.Category,
                Description = dto.Description,
                Price = dto.Price,
                DiscountPrice = dto.SalePrice,
                StockQuantity = dto.Stock,
                ImageUrl = dto.Image,
                Colors = dto.Colors,
                Sizes = dto.Sizes,
                IsNew = dto.IsNew,
                IsFeatured = dto.Featured,
                CategoryId = category.Id,
                Category = category
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, ToDto(product));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductDto>> Update(int id, ProductInputDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var category = await GetOrCreateCategory(dto.Type);

            product.Name = dto.Name;
            product.Brand = dto.Brand;
            product.Subcategory = dto.Category;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.DiscountPrice = dto.SalePrice;
            product.StockQuantity = dto.Stock;
            product.ImageUrl = dto.Image;
            product.Colors = dto.Colors;
            product.Sizes = dto.Sizes;
            product.IsNew = dto.IsNew;
            product.IsFeatured = dto.Featured;
            product.CategoryId = category.Id;
            product.Category = category;

            await _db.SaveChangesAsync();

            return ToDto(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            _db.Products.Remove(product);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "This product is referenced by existing cart, wishlist, or order records and cannot be deleted."
                });
            }

            return NoContent();
        }

        private async Task<Category> GetOrCreateCategory(string typeName)
        {
            var normalized = typeName.Trim();

            var category = await _db.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == normalized.ToLower());

            if (category != null) return category;

            category = new Category { Name = normalized };
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return category;
        }

        private static ProductDto ToDto(Product p)
        {
            var isOnSale = p.DiscountPrice.HasValue && p.DiscountPrice < p.Price;
            var discount = isOnSale
                ? (int)Math.Round((1 - (double)(p.DiscountPrice!.Value / p.Price)) * 100)
                : 0;

            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand,
                Type = p.Category.Name.ToLower(),
                Category = p.Subcategory,
                Description = p.Description,
                Image = p.ImageUrl,
                Price = p.Price,
                SalePrice = p.DiscountPrice,
                Discount = discount,
                Rating = p.Rating,
                Reviews = p.ReviewsCount,
                Colors = p.Colors,
                Sizes = p.Sizes,
                Stock = p.StockQuantity,
                IsNew = p.IsNew,
                IsOnSale = isOnSale,
                Featured = p.IsFeatured
            };
        }
    }
}
