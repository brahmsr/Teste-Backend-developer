using APItesteInside.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APItesteInside.Models.Entities
{
    public class Product
    {
        // Id do produto
        public int Id { get; set; }

        // Nome do produto
        public string Name { get; set; } = string.Empty;

        // Quantidade do produto
        public int Quantity { get; set; } = 0;

        // Valor do produto
        [DataType(DataType.Currency)]
        public decimal Price { get; set; } = 0;

        // Descrição do produto
        public string? Description { get; set; }

        // Categoria do produto
        public string? Category { get; set; }

        // Marcações de tempo
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<OrderProduct>? OrderProducts { get; set; }
    }
}
