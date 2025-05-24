using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedDate { get; set; } // << YENİ EKLENDİ
        public DateTime? ModifiedDate { get; set; } // << YENİ EKLENDİ
        public ICollection<ProductImageDto> ProductImages { get; set; } = new List<ProductImageDto>();
    }

    // Ürün oluşturmak için DTO
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [MaxLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, 999999.99, ErrorMessage = "Fiyat 0.01 ile 999999.99 arasında olmalıdır.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stok adedi zorunludur.")]
        [Range(0, 99999, ErrorMessage = "Stok adedi 0 ile 99999 arasında olmalıdır.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public Guid CategoryId { get; set; }

        public ICollection<ProductImageDto> ProductImages { get; set; } = new List<ProductImageDto>();
        public List<IFormFile>? NewImages { get; set; }
    }

    // Ürün güncellemek için DTO
    public class UpdateProductDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [MaxLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, 999999.99, ErrorMessage = "Fiyat 0.01 ile 999999.99 arasında olmalıdır.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stok adedi zorunludur.")]
        [Range(0, 99999, ErrorMessage = "Stok adedi 0 ile 99999 arasında olmalıdır.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public Guid CategoryId { get; set; }

        public List<string>? ExistingImagePaths { get; set; }
        public ICollection<ProductImageDto> ProductImages { get; set; } = new List<ProductImageDto>();
        public List<IFormFile>? NewImages { get; set; }
    }

    // ProductImageDto'nun da ayrı bir dosyada veya burada tanımlı olduğundan emin olun.
    // Eğer yoksa, bir önceki yanıttaki ProductImageDto yapısını buraya eklemelisiniz.
    // Örnek:
    // public class ProductImageDto
    // {
    //     public Guid Id { get; set; }
    //     public string ImagePath { get; set; }
    //     public Guid ProductId { get; set; }
    // }
}