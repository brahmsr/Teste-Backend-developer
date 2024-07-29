using System.ComponentModel.DataAnnotations;

namespace APItesteInside.DTOs
{
    public class ProductEditDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Category { get; set; } = string.Empty;
        [Required]
        public int Quantity { get; set; } = 0;
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; } = 0;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
