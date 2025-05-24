using System.Collections.Generic;
using System.Linq; // Sum için

namespace ECommerce.Application.DTOs
{
    public class CartDto
    {
        public Guid Id { get; set; } // Sepetin benzersiz ID'si (session ID veya kullanıcı ID'si olabilir)
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>(); // Sepetteki ürün öğeleri

        // Sepet toplam fiyatı
        public decimal GrandTotal => Items.Sum(item => item.TotalPrice);

        // Sepet öğe sayısı (farklı ürün adedi)
        public int ItemCount => Items.Count;

        // Sepetteki toplam ürün miktarı (tüm ürünlerin toplam adedi)
        public int TotalQuantity => Items.Sum(item => item.Quantity);
    }
}