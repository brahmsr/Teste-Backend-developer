namespace APItesteInside.Models.DTOs
{
    public class OrderChangeStatusDTO
    {
        public int status {  get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
