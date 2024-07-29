﻿using APItesteInside.Models.Entities;

namespace APItesteInside.DTOs
{
    public class OrderProductDTO
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int Quantity { get; set; }
    }
}