using ECommerce.Application.DTOs; // DTO'ları kullanacağız
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IProductService
    {
        // Eski GetAllProductsAsync metodu kaldırıldı veya değiştirildi
        // Task<List<ProductDto>> GetAllProductsAsync();

        // Yeni eklenen metot: Ürünleri filtreleme ve sayfalama özellikleri ile getirir.
        // ProductListDto, hem ürün listesini hem de sayfalama bilgilerini içerir.
        Task<ProductListDto> GetFilteredAndPaginatedProductsAsync(ProductFilterAndPaginationDto filterDto);

        Task<ProductDto> GetProductByIdAsync(Guid productId);
        Task<Guid> AddNewProductAsync(CreateProductDto productDto);
        Task UpdateProductAsync(UpdateProductDto productDto);
        Task DeleteProductAsync(Guid productId);
        // Daha fazla iş mantığı operasyonu eklenebilir (örneğin, stok güncelleme, indirim uygulama)
    }
}