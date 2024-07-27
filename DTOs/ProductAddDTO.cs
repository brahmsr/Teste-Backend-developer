using System.ComponentModel.DataAnnotations;

namespace APItesteInside.DTOs
{
    public class ProductAddDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
