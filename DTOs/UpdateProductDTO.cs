using System.ComponentModel.DataAnnotations;

namespace APItesteInside.DTOs
{
    public class UpdateProductDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; } = 0;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
