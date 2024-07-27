using static APItesteInside.DTOs.OrderAddDTO;

namespace APItesteInside.DTOs
{
    public class OrderEditDTO
    {
        public class EditOrdersDTO
        {
            public string OrderName { get; set; } = string.Empty;

            public decimal Price { get; set; } = 0;

            public string ClientName { get; set; } = string.Empty;

            public string Phone { get; set; } = string.Empty;

            public string? Email { get; set; } = string.Empty;

            public int status { get; set; }

            public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

            public ICollection<EditProductOrderDTO>? OrderProducts { get; set; }
        }

        public class EditProductOrderDTO
        {
            public int ProductId { get; set; }

            public int Quantity { get; set; }

        }
    }
}
