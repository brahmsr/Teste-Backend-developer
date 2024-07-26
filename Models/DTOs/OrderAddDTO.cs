using System.ComponentModel.DataAnnotations;

namespace APItesteInside.Models.DTOs
{
    public class OrderAddDTO
    {
        public class CreateOrderDTO
        {
            public string OrderName { get; set; } = string.Empty;

            public decimal Price { get; set; } = 0;

            public string ClientName { get; set; } = string.Empty;

            public string Phone { get; set; } = string.Empty;

            public string? Email { get; set; } = string.Empty;

            public int status { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public ICollection<CreateProductOrderDTO>? OrderProducts { get; set; }
        }

        public class CreateProductOrderDTO
        {

            public Guid ProductId { get; set; }

            public int Quantity { get; set; } = 0;
        }
    }
}
