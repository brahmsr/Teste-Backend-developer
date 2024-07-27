using APItesteInside.Models.Domain;

namespace APItesteInside.DTOs
{
    public class OrdersDTO
    {
        public Guid Id { get; set; }
        public string OrderName { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public string ClientName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public int status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public List<OrderProduct>? OrderProducts { get; set; }
    }

}
