using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // [Required], [MaxLength] ve [RegularExpression] için gerekli

namespace ECommerce.Application.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProductCount { get; set; } // Kategoriye ait ürün sayısı gibi ekstra bilgi
    }

    // Kategori oluşturmak için DTO
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [MaxLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        // Sadece harf, boşluk ve bazı özel karakterlere izin ver (Türkçe karakterler dahil)
        [RegularExpression(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s\-'.,&()]+$",
            ErrorMessage = "Kategori adı sadece harf, boşluk ve bazı özel karakterler (-,.',&,(),ğ,ü,ş,ı,ö,ç) içerebilir.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; } // Boş bırakılabilir olduğu için '?' eklendi
    }

    // Kategori güncellemek için DTO
    public class UpdateCategoryDto
    {
        [Required] // ID'nin zorunlu olduğunu belirtir
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [MaxLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir.")]
        // Sadece harf, boşluk ve bazı özel karakterlere izin ver (Türkçe karakterler dahil)
        [RegularExpression(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s\-'.,&()]+$",
            ErrorMessage = "Kategori adı sadece harf, boşluk ve bazı özel karakterler (-,.',&,(),ğ,ü,ş,ı,ö,ç) içerebilir.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; } // Boş bırakılabilir olduğu için '?' eklendi
    }
}