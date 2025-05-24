using System;
using System.Collections.Generic; // List için

namespace ECommerce.Application.DTOs
{
    public class CartItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; } // Sepette görünecek ana resim
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity; // Bu öğenin toplam fiyatı
    }
}