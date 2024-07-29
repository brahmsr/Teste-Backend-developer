using static APItesteInside.DTOs.OrderAddDTO;

namespace APItesteInside.DTOs
{
    public class OrderEditDTO
    {
        public string OrderName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public int status { get; set; } = 0;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
