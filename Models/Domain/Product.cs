using APItesteInside.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APItesteInside.Models.Entities
{
    public class Product
    {
        // Id do produto
        public Guid Id { get; set; }

        // Nome do produto
        [Required]
        public string Name { get; set; } = string.Empty;

        // Quantidade do produto
        [Required]
        public int Quantity { get; set; } = 0;

        // Valor do produto
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; } = 0;

        // Descrição do produto
        public string? Description { get; set; }

        // Categoria do produto
        public string? Category { get; set; }

        // Marcações de tempo
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
