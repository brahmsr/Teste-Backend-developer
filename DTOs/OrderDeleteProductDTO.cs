using static APItesteInside.DTOs.OrderEditDTO;

namespace APItesteInside.DTOs
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
            public int ProductId { get; set; }

            public int Quantity { get; set; }
        }
    }
}
