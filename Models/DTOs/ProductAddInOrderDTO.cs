using static APItesteInside.Models.DTOs.OrderEditDTO;

namespace APItesteInside.Models.DTOs
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
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
