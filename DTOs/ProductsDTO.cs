using APItesteInside.Models.Domain;
using APItesteInside.Models.Entities;

namespace APItesteInside.DTOs
{
    public class ProductsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Category { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public DateTime? CreatedAt {  get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<OrderProductsDTO>? OrderProducts { get; set; }

        public class OrderProductsDTO
        {
            public int OrderId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
