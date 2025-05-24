using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs
{
    public class AddToCartDto
    {
        [Required(ErrorMessage = "Ürün ID'si zorunludur.")]
        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Miktar en az 1 olmalıdır.")]
        public int Quantity { get; set; } = 1; // Varsayılan miktar 1
    }
}