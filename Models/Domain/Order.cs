using APItesteInside.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace APItesteInside.Models.Entities
{
    public class Order
    {
        // Id do pedido
        [Key]
        public Guid Id { get; set; }

        // Nome do Pedido
        [Required]
        public string OrderName { get; set; }

        // Preço do produto
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        // Nome do Cliente
        [Required]
        public string ClientName { get; set; }

        // Telefone do Cliente 
        [Required]
        public string Phone { get; set; }

        // Email do cliente
        public string? Email { get; set; }

        // Status do pedido 0=aberto 1=fechado 2=cancelado
        [Required]
        public int status { get; set; }

        // Marcações de tempo
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
