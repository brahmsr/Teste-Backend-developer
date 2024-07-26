using static APItesteInside.Models.DTOs.OrderEditDTO;

namespace APItesteInside.Models.DTOs
{
    public class OrderDeleteProductDTO
    {
        public class RemoveItemInOrderDTO
        {
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

            public ICollection<RemoveProductOrderDTO>? OrderProducts { get; set; }
        }

        public class RemoveProductOrderDTO
        {
            public Guid ProductId { get; set; }

            public int Quantity { get; set; }
        }
    }
}
