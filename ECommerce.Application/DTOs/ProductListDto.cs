using System.Collections.Generic; // List<T> kullanmak için
using System; // Guid için

namespace ECommerce.Application.DTOs
{
    // Ürün listesi ve sayfalama bilgilerini bir arada taşıyan DTO
    public class ProductListDto
    {
        public List<ProductDto> Products { get; set; } // Ürünlerin listesi
        public int PageNumber { get; set; } // Mevcut sayfa numarası
        public int PageSize { get; set; } // Bir sayfadaki ürün sayısı
        public int TotalCount { get; set; } // Filtrelenmiş toplam ürün sayısı
        public int TotalPages { get; set; } // Toplam sayfa sayısı (TotalCount'a göre hesaplanır)

        // *** YENİ EKLENEN FİLTRELEME ALANLARI ***
        public string SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}