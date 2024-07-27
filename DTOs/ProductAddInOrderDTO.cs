using static APItesteInside.DTOs.OrderEditDTO;

namespace APItesteInside.DTOs
{
    public class ProductAddInOrderDTO
    {
        public class OrderAddProdDTO
        {
            public DateTime UpdatedAt = DateTime.Now;
            public int status { get; set; }
            public ICollection<AddedProductDTO>? OrderProducts { get; set; }
        }
        public class AddedProductDTO
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
