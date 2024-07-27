using System.ComponentModel.DataAnnotations;

namespace APItesteInside.DTOs
{
    public class UpdateProductQuantityDTO
    {
        [Required]
        public int Quantity { get; set; }
    }
}
