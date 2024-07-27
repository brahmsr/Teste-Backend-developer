using APItesteInside.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace APItesteInside.Models.Entities
{
    public class Order
    {
        public int Id { get; set; }

        // Nome do Pedido
        public string OrderName { get; set; }

        // Preço do produto

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        // Nome do Cliente
        public string ClientName { get; set; }

        // Telefone do Cliente 
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

        public List<OrderProduct>? OrderProducts { get; set; }
    }
}
