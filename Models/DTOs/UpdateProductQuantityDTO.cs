using System.ComponentModel.DataAnnotations;

namespace APItesteInside.Models.DTOs
{
    public class UpdateProductQuantityDTO
    {
        [Required]
        public int Quantity { get; set; }
    }
}
