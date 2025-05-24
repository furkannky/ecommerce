using System;

namespace ECommerce.Application.DTOs
{
    // Ürün filtreleme ve sayfalama kriterlerini taşıyan DTO
    public class ProductFilterAndPaginationDto
    {
        public string SearchTerm { get; set; } // Ürün adı veya açıklamada aranacak kelime
        public Guid? CategoryId { get; set; } // Filtrelenecek kategori ID'si (null olabilir)
        public decimal? MinPrice { get; set; } // Minimum fiyat
        public decimal? MaxPrice { get; set; } // Maksimum fiyat

        public int PageNumber { get; set; } = 1; // Hangi sayfa gösterilecek (varsayılan 1)
        public int PageSize { get; set; } = 12; // Bir sayfada kaç ürün olacak (varsayılan 12)

        public string OrderBy { get; set; } // << YENİ EKLENDİ: Sıralama kriteri (örn: "DateDescending", "PriceAscending")

        // Bu alanlar servis tarafında doldurulacak
        public int TotalCount { get; set; } // Toplam ürün sayısı (servis tarafından doldurulacak)
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize); // Toplam sayfa sayısı (TotalCount'a göre hesaplanır)
    }
}